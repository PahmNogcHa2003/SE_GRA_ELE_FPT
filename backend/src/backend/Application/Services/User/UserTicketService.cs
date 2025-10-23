using Application.DTOs.Tickets;
using Application.Interfaces.Base;
using Application.Interfaces.User.Repository;
using Application.Interfaces.User.Service;
using Application.Interfaces;
using Application.Services.Base;
using AutoMapper;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

public class UserTicketService
    : GenericService<UserTicket, UserTicketDTO, long>, IUserTicketService
{
    private readonly IUnitOfWork _uow;
    private readonly IWalletService _walletService;
    private readonly IMapper _mapper;

    private readonly IRepository<TicketPlanPrice, long> _planPriceRepo;
    private readonly IRepository<Order, long> _orderRepo;
    private readonly IRepository<Wallet, long> _walletRepo;
    private readonly IRepository<WalletTransaction, long> _walletTxnRepo;
    private readonly IUserTicketRepository _userTicketRepository;

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
        _walletService = walletService;
        _mapper = mapper;
        _userTicketRepository = userTicketRepository;
        _planPriceRepo = planPriceRepo;
        _orderRepo = orderRepo;
        _walletRepo = walletRepo;
        _walletTxnRepo = walletTxnRepo;
    }

    public async Task<UserTicketDTO?> PurchaseTicketAsync(long? userId, PurchaseTicketRequestDTO request, CancellationToken ct)
    {
        // 1) Lấy PlanPrice (active) + tên Plan
        var planPrice = await _planPriceRepo.Query()
            .Include(x => x.Plan)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == request.PlanPriceId && p.IsActive, ct);

        if (planPrice is null)
            throw new KeyNotFoundException("Loại vé không hợp lệ hoặc đã hết hạn.");

        // 2) Nếu là gói thuê bao, chặn mua trùng khi còn hiệu lực/chờ
        if (planPrice.ValidityDays.HasValue && planPrice.ValidityDays.Value > 0)
        {
            var hasExisting = await _userTicketRepository
                .HasExistingSubscriptionAsync(userId.Value, request.PlanPriceId, ct);
            if (hasExisting)
                throw new InvalidOperationException("Bạn đã có gói vé tương tự đang hoạt động hoặc đang chờ kích hoạt.");
        }

        // 3) Lấy ví của user
        var wallet = await _walletRepo.Query()
            .FirstOrDefaultAsync(w => w.UserId == userId && w.Status == "Active", ct);
        if (wallet is null)
            throw new InvalidOperationException("Ví không tồn tại hoặc không hoạt động.");

        // 4) Tính tiền đơn hàng
        var subtotal = planPrice.Price;
        var discount = 0m;
        var total = subtotal - discount;
        if (total < 0) total = 0;

        // 5) Kiểm tra số dư ví
        if (wallet.Balance < total)
            throw new InvalidOperationException("Số dư ví không đủ để mua gói.");

        var now = DateTimeOffset.UtcNow;
        var trx = await _uow.BeginTransactionAsync(ct);
        try
        {
            // 6) Tạo Order (Pending)
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
                CreatedAt = now
            };
            await _orderRepo.AddAsync(order, ct);


            // 7) Trừ ví (cập nhật Wallet + ghi WalletTransaction)
            wallet.Balance -= total;
            wallet.UpdatedAt = now;
            _walletRepo.Update(wallet);

            var walletTxn = new WalletTransaction
            {
                WalletId = wallet.Id,
                Direction = "Out",             // theo schema: 6 ký tự
                Source = "TicketPurchase",    // nguồn: mua vé
                Amount = total,
                BalanceAfter = wallet.Balance,
                CreatedAt = now
            };
            await _walletTxnRepo.AddAsync(walletTxn, ct);

            // 8) Tạo UserTicket (Ready)
            var newUserTicket = new UserTicket
            {
                UserId = userId.Value,
                PlanPriceId = planPrice.Id,
                PurchasedPrice = total,
                Status = "Ready",
                CreatedAt = now,
                RemainingMinutes = planPrice.DurationLimitMinutes,
                RemainingRides = null,
                SerialCode = GenerateSerialCode(userId.Value, planPrice.Id)
                // ActivatedAt/ExpiresAt: để null, sẽ set khi kích hoạt
            };
            await _userTicketRepository.AddAsync(newUserTicket, ct);

            // 9) Cập nhật Order sang Paid
            order.Status = "Paid";
            order.PaidAt = now;
            _orderRepo.Update(order);

            // 10) Lưu & Commit
            await _uow.SaveChangesAsync(ct);
            await _uow.CommitTransactionAsync(trx, ct);

            // 11) Trả DTO
            var dto = _mapper.Map<UserTicketDTO>(newUserTicket);
            dto.PlanName = planPrice.Plan?.Name;
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
        var tickets = await _userTicketRepository.GetActiveTicketsByUserIdAsync(userId, ct);

        return tickets.Select(ut => new UserTicketDTO
        {
            Id = ut.Id,
            PlanPriceId = ut.PlanPriceId,
            PlanName = $"{ut.PlanPrice.Plan.Name} - {ut.PlanPrice.VehicleType}",
            SerialCode = ut.SerialCode,
            PurchasedPrice = ut.PurchasedPrice,
            Status = ut.Status,
            ActivatedAt = ut.ActivatedAt,
            ExpiresAt = ut.ExpiresAt,
            RemainingMinutes = ut.RemainingMinutes,
            CreatedAt = ut.CreatedAt
        }).ToList();
    }

    private static string GenerateOrderNo(long userId)
        => $"ORD-{userId}-{DateTime.UtcNow:yyyyMMddHHmmss}";

    private static string GenerateSerialCode(long userId, long planPriceId)
        => $"USR-{userId}-PP-{planPriceId}-{DateTime.UtcNow:yyyyMMddHHmmss}";
}
