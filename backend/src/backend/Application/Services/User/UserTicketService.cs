using Application.DTOs.Tickets;
using Application.Interfaces.Base;
using Application.Interfaces.User.Repository;
using Application.Interfaces.User.Service;
using Application.Interfaces;
using Application.Services.Base;
using AutoMapper;
using Domain.Entities;
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
        IUserTicketRepository userTicketRepository
    ) : base(userTicketRepo, mapper, uow)
    {
        _uow = uow;
        _mapper = mapper;
        _userTicketRepository = userTicketRepository;
        _planPriceRepo = planPriceRepo;
        _orderRepo = orderRepo;
        _walletRepo = walletRepo;
        _walletTxnRepo = walletTxnRepo;
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

    public async Task<UserTicketDTO?> PurchaseTicketAsync(long? userId, PurchaseTicketRequestDTO request, CancellationToken ct)
    {
        if (userId is null || userId <= 0) throw new InvalidOperationException("User không hợp lệ.");
        if (request is null) throw new ArgumentNullException(nameof(request));

        var planPrice = await _planPriceRepo.Query()
            .Include(x => x.Plan)
            .FirstOrDefaultAsync(p => p.Id == request.PlanPriceId && p.IsActive, ct);

        if (planPrice is null)
            throw new KeyNotFoundException("Loại vé không hợp lệ hoặc đã ngừng bán.");

        var nowUtc = DateTimeOffset.UtcNow;
        var isOnFirstUse = planPrice.ActivationMode == PlanActivationMode.ON_FIRST_USE;
        var isSubscription = planPrice.ValidityDays.GetValueOrDefault() > 0; // Day/Month

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
                throw new InvalidOperationException("Bạn đang có gói theo thời gian còn hiệu lực. Vui lòng dùng hết/đợi hết hạn rồi mua gói mới.");
        }

        // Ví
        var wallet = await _walletRepo.Query()
            .FirstOrDefaultAsync(w => w.UserId == userId.Value && w.Status == "Active", ct);
        if (wallet is null)
            throw new InvalidOperationException("Ví không tồn tại hoặc không hoạt động.");

        var subtotal = planPrice.Price;
        var discount = 0m;
        var total = Math.Max(0, subtotal - discount);

        if (wallet.Balance < total)
            throw new InvalidOperationException("Số dư ví không đủ để mua gói.");

        var nowLocal = TimeZoneInfo.ConvertTime(nowUtc, VnTz);
        var trx = await _uow.BeginTransactionAsync(ct);

        try
        {
            // Order (Pending)
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

            // Trừ ví & ghi giao dịch
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

            // Tạo vé
            var newTicket = new UserTicket
            {
                UserId = userId.Value,
                PlanPriceId = planPrice.Id,
                PurchasedPrice = total,
                SerialCode = GenerateSerialCode(userId.Value, planPrice.Id),
                Status = "Ready",
                CreatedAt = nowUtc,
                RemainingMinutes = planPrice.DurationLimitMinutes,
                RemainingRides = null
            };

            if (isOnFirstUse)
            {
                // Vé lượt: để dành, có hạn kích hoạt
                newTicket.ActivationDeadline = nowUtc.AddDays(planPrice.ActivationWindowDays ?? 30);
                newTicket.RemainingRides = 1;
            }
            else
            {
                // Subscription: kích hoạt ngay
                newTicket.Status = "Active";
                newTicket.ActivatedAt = nowUtc;

                if (string.Equals(planPrice.Plan.Type, "Day", StringComparison.OrdinalIgnoreCase))
                {
                    // Ngày theo VN, lưu UTC
                    var startLocal = new DateTimeOffset(nowLocal.Year, nowLocal.Month, nowLocal.Day, 0, 0, 0, VnTz.GetUtcOffset(nowLocal));
                    var endLocal = startLocal.AddDays(1).AddTicks(-1);

                    newTicket.ValidFrom = startLocal.ToUniversalTime();
                    newTicket.ValidTo = endLocal.ToUniversalTime();
                }
                else // Month
                {
                    var days = planPrice.ValidityDays ?? 30;
                    newTicket.ValidFrom = nowUtc;
                    newTicket.ValidTo = nowUtc.AddDays(days);
                }

                // Đồng bộ để mọi loại vé đều có ExpiresAt
                newTicket.ExpiresAt = newTicket.ValidTo;
            }

            await _userTicketRepository.AddAsync(newTicket, ct);

            // Hoàn tất order
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
