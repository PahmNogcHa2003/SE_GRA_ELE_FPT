using Application.DTOs.Tickets;
using Application.Interfaces;
using Application.Interfaces.Base;
using Application.Interfaces.Staff.Repository;
using Application.Interfaces.User.Repository;
using Application.Interfaces.User.Service;
using Application.Services.Base;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;

public class UserTicketService
    : GenericService<UserTicket, UserTicketDTO, long>, IUserTicketService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    private readonly IRepository<TicketPlanPrice, long> _planPriceRepo;
    private readonly IRepository<Order, long> _orderRepo;
    private readonly IRepository<Wallet, long> _walletRepo;
    private readonly IRepository<WalletTransaction, long> _walletTxnRepo;
    private readonly IUserTicketRepository _userTicketRepository;
    private readonly IVoucherRepository _voucherRepository;
    private readonly IVoucherUsageRepository _voucherUsageRepository;

    private static readonly TimeZoneInfo VnTz =
        TimeZoneInfo.FindSystemTimeZoneById(
            RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                ? "SE Asia Standard Time"
                : "Asia/Ho_Chi_Minh");

    public UserTicketService(
        IRepository<UserTicket, long> userTicketRepo,
        IRepository<TicketPlanPrice, long> planPriceRepo,
        IRepository<Order, long> orderRepo,
        IRepository<Wallet, long> walletRepo,
        IRepository<WalletTransaction, long> walletTxnRepo,
        IUnitOfWork uow,
        IWalletService walletService,
        IMapper mapper,
        IUserTicketRepository userTicketRepository,
        IVoucherRepository voucherRepository,
        IVoucherUsageRepository voucherUsageRepository
    ) : base(userTicketRepo, mapper, uow)
    {
        _uow = uow;
        _mapper = mapper;
        _userTicketRepository = userTicketRepository;
        _planPriceRepo = planPriceRepo;
        _orderRepo = orderRepo;
        _walletRepo = walletRepo;
        _walletTxnRepo = walletTxnRepo;
        _voucherRepository = voucherRepository;
        _voucherUsageRepository = voucherUsageRepository;   
    }

    // ===== Helper: hạn thực tế của vé
    private static DateTimeOffset? EffectiveExpiry(UserTicket t)
        => t.ValidTo ?? t.ExpiresAt;

    // ===== On-demand: cập nhật trạng thái Expired
    private async Task EnsureTicketStatusesUpToDateAsync(long userId, CancellationToken ct)
    {
        var now = DateTimeOffset.UtcNow;

        var list = await _userTicketRepository.Query()
            .Where(t => t.UserId == userId && t.Status != "Expired")
            .ToListAsync(ct);

        foreach (var t in list)
        {
            // Vé lượt để dành quá hạn kích hoạt
            if (t.Status == "Ready" && t.ActivationDeadline != null && t.ActivationDeadline <= now)
                t.Status = "Expired";

            // Subscription quá hạn hiệu lực
            var exp = EffectiveExpiry(t);
            if (exp != null && exp <= now)
                t.Status = "Expired";
        }

        await _uow.SaveChangesAsync(ct);
    }

    public async Task<List<UserTicketPlanDTO>> GetTicketMarketAsync(string? vehicleType, CancellationToken ct)
    {
        var q = _planPriceRepo.Query()
            .Include(p => p.Plan)
            .Where(p => p.IsActive);

        if (!string.IsNullOrWhiteSpace(vehicleType))
        {
            var vt = vehicleType.ToLower();
            q = q.Where(p => p.VehicleType != null && p.VehicleType.ToLower() == vt);
        }

        var prices = await q.AsNoTracking().ToListAsync(ct);

        var groups = prices.GroupBy(p => p.Plan);
        var result = new List<UserTicketPlanDTO>();

        foreach (var g in groups)
        {
            var planDto = _mapper.Map<UserTicketPlanDTO>(g.Key);
            planDto.Prices = _mapper.Map<List<UserTicketPlanPriceDTO>>(g.ToList());
            planDto.Code = g.Key.Code;
            planDto.Type = g.Key.Type;
            result.Add(planDto);
        }
        return result.OrderBy(x => x.Id).ToList();
    }

    public async Task<UserTicketDTO?> PurchaseTicketAsync(
    long? userId,
    PurchaseTicketRequestDTO request,
    CancellationToken ct)
    {
        if (userId is null || userId <= 0)
            throw new InvalidOperationException("User không hợp lệ.");
        if (request is null)
            throw new ArgumentNullException(nameof(request));

        var planPrice = await _planPriceRepo.Query()
            .Include(x => x.Plan)
            .FirstOrDefaultAsync(p => p.Id == request.PlanPriceId && p.IsActive, ct);

        if (planPrice is null)
            throw new KeyNotFoundException("Loại vé không hợp lệ hoặc đã ngừng bán.");

        var nowUtc = DateTimeOffset.UtcNow;

        var isOnFirstUse = planPrice.ActivationMode == PlanActivationMode.ON_FIRST_USE;
        var isSubscription = planPrice.ValidityDays.GetValueOrDefault() > 0;

        // ===== Check subscription overlap =====
        if (isSubscription)
        {
            var hasAnyActiveSubscription = await _userTicketRepository.Query()
                .Include(t => t.PlanPrice)
                .Where(t => t.UserId == userId.Value)
                .Where(t => t.PlanPrice.ValidityDays > 0)
                .Where(t => (t.ValidTo ?? t.ExpiresAt) != null && (t.ValidTo ?? t.ExpiresAt) > nowUtc)
                .Where(t => t.Status == "Active" || t.Status == "Ready")
                .AnyAsync(ct);

            if (hasAnyActiveSubscription)
                throw new InvalidOperationException("Bạn đang có gói theo thời gian còn hiệu lực.");
        }

        // ===== Lấy ví user =====
        var wallet = await _walletRepo.Query()
            .FirstOrDefaultAsync(w => w.UserId == userId.Value && w.Status == "Active", ct);

        if (wallet is null)
            throw new InvalidOperationException("Ví không tồn tại hoặc không hoạt động.");

        // ==========================================================
        // ===============   CHECK & APPLY VOUCHER   ===============
        // ==========================================================
        Voucher? voucher = null;
        decimal discount = 0m;
        var subtotal = planPrice.Price;

        if (!string.IsNullOrWhiteSpace(request.VoucherCode))
        {
            voucher = await _voucherRepository.Query()
                .FirstOrDefaultAsync(v => v.Code == request.VoucherCode, ct);

            if (voucher == null)
                throw new InvalidOperationException("Voucher không tồn tại.");

            var now = DateTimeOffset.UtcNow;

            // Status
            if (voucher.Status != VoucherStatus.Active)
                throw new InvalidOperationException("Voucher không hoạt động.");

            // Thời gian
            if (voucher.StartDate > now)
                throw new InvalidOperationException("Voucher chưa tới thời gian sử dụng.");
            if (voucher.EndDate < now)
                throw new InvalidOperationException("Voucher đã hết hạn.");

            // Tổng số lượt dùng
            if (voucher.UsageLimit.HasValue)
            {
                var totalUsed = await _voucherUsageRepository.Query()
                    .CountAsync(x => x.VoucherId == voucher.Id, ct);

                if (totalUsed >= voucher.UsageLimit.Value)
                    throw new InvalidOperationException("Voucher đã hết lượt sử dụng.");
            }

            // Lượt theo user
            if (voucher.UsagePerUser.HasValue)
            {
                var usedByUser = await _voucherUsageRepository.Query()
                    .CountAsync(x => x.UserId == userId && x.VoucherId == voucher.Id, ct);

                if (usedByUser >= voucher.UsagePerUser.Value)
                    throw new InvalidOperationException("Bạn đã dùng hết số lượt cho voucher này.");
            }

            // Giá tối thiểu
            if (voucher.MinOrderAmount.HasValue && planPrice.Price < voucher.MinOrderAmount.Value)
                throw new InvalidOperationException(
                    $"Đơn tối thiểu để dùng voucher là {voucher.MinOrderAmount:N0}.");

            // ===== Tính discount =====
            if (voucher.IsPercentage)
            {
                discount = subtotal * (voucher.Value / 100m);
                if (voucher.MaxDiscountAmount.HasValue)
                    discount = Math.Min(discount, voucher.MaxDiscountAmount.Value);
            }
            else
            {
                discount = voucher.Value;
                if (voucher.MaxDiscountAmount.HasValue)
                    discount = Math.Min(discount, voucher.MaxDiscountAmount.Value);
            }

            discount = Math.Min(discount, subtotal);
        }

        var total = subtotal - discount;

        // ===== Kiểm tra số dư ví =====
        if (wallet.Balance < total)
            throw new InvalidOperationException("Số dư ví không đủ để mua gói.");

        var nowLocal = TimeZoneInfo.ConvertTime(nowUtc, VnTz);

        // ===== Transaction =====
        var trx = await _uow.BeginTransactionAsync(ct);

        try
        {
            // ===== Tạo Order (Pending) =====
            var order = new Order
            {
                UserId = userId.Value,
                OrderNo = GenerateOrderNo(userId.Value),
                OrderType = "TicketPurchase",
                Status = "Pending",
                Subtotal = subtotal,
                Discount = discount,
                Total = total,
                Currency = wallet.Currency ?? "VND",
                CreatedAt = nowUtc
            };
            await _orderRepo.AddAsync(order, ct);

            // ===== Trừ ví =====
            wallet.Balance -= total;
            wallet.UpdatedAt = nowUtc;
            _walletRepo.Update(wallet);

            var walletTxn = new WalletTransaction
            {
                WalletId = wallet.Id,
                Direction = "Out",
                Source = "TicketPurchase",
                Amount = total,
                BalanceAfter = wallet.Balance,
                CreatedAt = nowUtc
            };
            await _walletTxnRepo.AddAsync(walletTxn, ct);

            // ===== Tạo vé cho user =====
            var newTicket = new UserTicket
            {
                UserId = userId.Value,
                PlanPriceId = planPrice.Id,
                PurchasedPrice = total,
                SerialCode = GenerateSerialCode(userId.Value, planPrice.Id),
                Status = isOnFirstUse ? "Ready" : "Active",
                CreatedAt = nowUtc,
                RemainingMinutes = planPrice.DurationLimitMinutes,
                RemainingRides = isOnFirstUse ? 1 : null
            };

            if (isOnFirstUse)
                newTicket.ActivationDeadline = nowUtc.AddDays(planPrice.ActivationWindowDays ?? 30);
            else
            {
                newTicket.ActivatedAt = nowUtc;

                if (string.Equals(planPrice.Plan.Type, "Day", StringComparison.OrdinalIgnoreCase))
                {
                    var startLocal = new DateTimeOffset(
                        nowLocal.Year, nowLocal.Month, nowLocal.Day,
                        0, 0, 0,
                        VnTz.GetUtcOffset(nowLocal));

                    var endLocal = startLocal.AddDays(1).AddTicks(-1);

                    newTicket.ValidFrom = startLocal.ToUniversalTime();
                    newTicket.ValidTo = endLocal.ToUniversalTime();
                }
                else
                {
                    var days = planPrice.ValidityDays ?? 30;
                    newTicket.ValidFrom = nowUtc;
                    newTicket.ValidTo = nowUtc.AddDays(days);
                }

                newTicket.ExpiresAt = newTicket.ValidTo;
            }

            await _userTicketRepository.AddAsync(newTicket, ct);

            // ===== Lưu VoucherUsage =====
            if (voucher != null)
            {
                var usage = new VoucherUsage
                {
                    VoucherId = voucher.Id,
                    UserId = userId.Value,
                    TicketPlanPriceId = planPrice.Id,
                    UsedAt = nowUtc
                };
                await _voucherUsageRepository.AddAsync(usage, ct);
            }

            // ===== Hoàn tất order =====
            order.Status = "Paid";
            order.PaidAt = nowUtc;

            await _uow.SaveChangesAsync(ct);
            await _uow.CommitTransactionAsync(trx, ct);

            var dto = _mapper.Map<UserTicketDTO>(newTicket);
            dto.PlanName = planPrice.Plan?.Name ?? string.Empty;
            dto.ActivationMode = (ActivationModeDTO)(int)planPrice.ActivationMode;

            return dto;
        }
        catch
        {
            await _uow.RollbackTransactionAsync(trx, ct);
            throw;
        }
    }

    public async Task<List<UserTicketDTO>?> GetMyActiveTicketsAsync(long? userId, CancellationToken ct)
    {
        if (userId is null || userId <= 0) return new List<UserTicketDTO>();

        await EnsureTicketStatusesUpToDateAsync(userId.Value, ct);

        var now = DateTimeOffset.UtcNow;

        var data = await _userTicketRepository.Query()
            .Include(x => x.PlanPrice).ThenInclude(pp => pp.Plan)
            .Where(x => x.UserId == userId.Value)
            .Where(x =>
                // Subscription còn hạn
                (x.Status == "Active" && (x.ValidTo ?? x.ExpiresAt) != null && (x.ValidTo ?? x.ExpiresAt) > now)
                // Vé lượt đang để dành còn hạn kích hoạt
                || (x.Status == "Ready" && (x.ActivationDeadline == null || x.ActivationDeadline > now)))
            .OrderByDescending(x => x.CreatedAt)
            .AsNoTracking()
            .Select(ut => new UserTicketDTO
            {
                Id = ut.Id,
                PlanPriceId = ut.PlanPriceId,
                PlanName = ut.PlanPrice.Plan != null ? ut.PlanPrice.Plan.Name : string.Empty,
                VehicleType = ut.PlanPrice.VehicleType!, 
                SerialCode = ut.SerialCode,
                PurchasedPrice = ut.PurchasedPrice,
                Status = ut.Status,
                ActivatedAt = ut.ActivatedAt,
                ValidFrom = ut.ValidFrom,
                ValidTo = ut.ValidTo,
                ExpiresAt = ut.ExpiresAt,
                ActivationDeadline = ut.ActivationDeadline,
                RemainingMinutes = ut.RemainingMinutes,
                RemainingRides = ut.RemainingRides,
                CreatedAt = ut.CreatedAt,
                ActivationMode = (ActivationModeDTO)(int)ut.PlanPrice.ActivationMode
            })
            .ToListAsync(ct);

        return data;
    }

    public async Task<UserTicketDTO?> GetIdByUserIdAsync(long? userId, long ticketId, CancellationToken ct)
    {
        var ut = await _userTicketRepository.Query()
            .Include(t => t.PlanPrice).ThenInclude(pp => pp.Plan)
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == ticketId && t.UserId == userId, ct);

        if (ut is null) return null;

        var dto = new UserTicketDTO
        {
            Id = ut.Id,
            PlanPriceId = ut.PlanPriceId,
            PlanName = $"{ut.PlanPrice.Plan?.Name} - {ut.PlanPrice.VehicleType}",
            SerialCode = ut.SerialCode,
            PurchasedPrice = ut.PurchasedPrice,
            Status = ut.Status,

            ActivatedAt = ut.ActivatedAt,
            ValidFrom = ut.ValidFrom,
            ValidTo = ut.ValidTo,
            ExpiresAt = ut.ExpiresAt,
            ActivationDeadline = ut.ActivationDeadline,
            RemainingMinutes = ut.RemainingMinutes,
            RemainingRides = ut.RemainingRides,
            CreatedAt = ut.CreatedAt,

            ActivationMode = (ActivationModeDTO)(int)ut.PlanPrice.ActivationMode
        };

        return dto;
    }

    private static string GenerateOrderNo(long userId)
        => $"ORD-{userId}-{DateTime.UtcNow:yyyyMMddHHmmss}";

    private static string GenerateSerialCode(long userId, long planPriceId)
        => $"USR-{userId}-PP-{planPriceId}-{DateTime.UtcNow:yyyyMMddHHmmss}";
}
