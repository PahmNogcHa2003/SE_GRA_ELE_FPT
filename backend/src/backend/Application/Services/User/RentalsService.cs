using Application.DTOs.Rental;
using Application.Exceptions;
using Application.Helpers;
using Application.Interfaces;
using Application.Interfaces.Staff.Repository;
using Application.Interfaces.User.Repository;
using Application.Interfaces.User.Service;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using Application.Interfaces.Base;
using Application.DTOs.RentalHistory;

namespace Application.Services.User
{
    public class RentalsService : IRentalsService
    {
        private readonly IStationsRepository _stationRepo;
        private readonly IVehicleRepository _vehicleRepo;
        private readonly IUserTicketRepository _userTicketRepository;
        private readonly IRentalsRepository _rentalRepo;
        private readonly IBookingTicketRepository _bookingTicketRepo;
        private readonly IRentalHistoryRepository _rentalHistoryRepo;
        private readonly IWalletRepository _walletRepo;
        private readonly IWalletDebtRepository _walletDebtRepo;
        private readonly IOrderRepository _orderRepo;
        private readonly IRepository<TicketPlanPrice, long> _planPriceRepo;
        private readonly IUnitOfWork _uow;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<RentalsService> _logger;
        private readonly IUserLifetimeStatsService _userLifetimeStatsService;
        private readonly IQuestService _questService;
        private static DateTime NowUtc() => DateTime.UtcNow;

        private static DateTime NowVn()
        {
            return NowUtc().AddHours(7);
        }
        private static DateTimeOffset ToVn(DateTimeOffset utcTime)
        {
            return utcTime.AddHours(7);
        }

        public RentalsService(
            IStationsRepository stationRepo,
            IVehicleRepository vehicleRepo,
            IUserTicketRepository userTicketRepository,
            IRentalsRepository rentalRepo,
            IBookingTicketRepository bookingTicketRepo,
            IRentalHistoryRepository rentalHistoryRepo,
            IWalletRepository walletRepo,
            IWalletDebtRepository walletDebtRepo,
            IOrderRepository orderRepo,
            IRepository<TicketPlanPrice, long> planPriceRepo,
            IUnitOfWork uow,
            IHttpContextAccessor httpContextAccessor,
            IQuestService questService,
            IUserLifetimeStatsService userLifetimeStatsService,
            ILogger<RentalsService> logger)
        {
            _stationRepo = stationRepo;
            _vehicleRepo = vehicleRepo;
            _userTicketRepository = userTicketRepository;
            _rentalRepo = rentalRepo;
            _bookingTicketRepo = bookingTicketRepo;
            _rentalHistoryRepo = rentalHistoryRepo;
            _walletRepo = walletRepo;
            _walletDebtRepo = walletDebtRepo;
            _orderRepo = orderRepo;
            _planPriceRepo = planPriceRepo;
            _userLifetimeStatsService = userLifetimeStatsService;
            _questService = questService;
            _uow = uow;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        private long GetCurrentUserIdOrThrow()
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim) || !long.TryParse(userIdClaim, out long userId))
                throw new UnauthorizedAccessException("User chưa đăng nhập.");

            return userId;
        }

        private static DateTimeOffset? EffectiveExpiry(UserTicket t)
            => t.ValidTo ?? t.ExpiresAt;

        private static string GenerateOvertimeOrderNo(long userId)
            => $"OVT-{userId}-{DateTime.UtcNow:yyyyMMddHHmmss}";

        // CREATE RENTAL
        public async Task<long> CreateRentalAsync(CreateRentalDTO dto, CancellationToken ct = default)
        {
            var userId = GetCurrentUserIdOrThrow();
            var unpaidDebts = await _walletDebtRepo.HasUnpaidDebtsAsync(userId, ct);
            if (unpaidDebts)
            {
                throw new BadRequestException(
                    "Bạn đang có khoản nợ chưa thanh toán. " +
                    "Vui lòng thanh toán nợ trong ví trước khi tiếp tục thuê xe.");
            }

            // Không cho user có nhiều chuyến đang chạy
            var hasOngoing = await _rentalRepo.Query()
                .AnyAsync(r => r.UserId == userId && r.Status == RentalStatus.Ongoing, ct);
            if (hasOngoing)
                throw new BadRequestException("Bạn đang có một chuyến đi chưa kết thúc.");

            // Lấy trạm hiện tại của xe
            var startStationId = await _vehicleRepo.GetStationVehicle(dto.VehicleId);
            if (startStationId == 0)
                throw new NotFoundException("Xe không tồn tại hoặc không ở trạm nào.");

            // Kiểm tra xe
            var vehicle = await _vehicleRepo.GetVehicleWithCategoryAsync(dto.VehicleId);
            if (vehicle == null)
                throw new NotFoundException("Không tìm thấy thông tin loại xe.");

            if (vehicle.Status == VehicleStatus.InUse)
                throw new BadRequestException("Xe này đang được sử dụng.");

            var nowUtc = NowUtc();
            var nowVn = NowVn(); // nếu sau này cần hiển thị/log

            // Lấy vé người dùng chọn
            var userTicket = await _userTicketRepository.GetByIdAsync(dto.UserTicketId);
            if (userTicket == null)
                throw new NotFoundException("Không tìm thấy vé đã chọn.");

            if (userTicket.UserId != userId)
                throw new BadRequestException("Vé không thuộc về người dùng hiện tại.");

            var planPrice = await _planPriceRepo.Query()
                .Include(p => p.Plan)
                .FirstOrDefaultAsync(p => p.Id == userTicket.PlanPriceId, ct);

            if (planPrice == null || !planPrice.IsActive)
                throw new BadRequestException("Loại vé hiện không còn hoạt động.");

            var exp = EffectiveExpiry(userTicket);
            if (exp.HasValue && exp.Value <= nowUtc)
                throw new BadRequestException("Vé đã hết hạn.");

            var isSubscription = planPrice.ValidityDays.GetValueOrDefault() > 0; // vé ngày / tháng
            var isTripPass = !isSubscription;                                // vé lượt

            // ===== Kích hoạt vé nếu đang Ready =====
            if (userTicket.Status == UserTicketStatus.Ready)
            {
                // Vé lượt: có hạn kích hoạt
                if (userTicket.ActivationDeadline.HasValue && userTicket.ActivationDeadline <= nowUtc)
                    throw new BadRequestException("Vé đã quá thời hạn kích hoạt.");

                userTicket.Status = UserTicketStatus.Active;
                userTicket.ActivatedAt = nowUtc;

                // Subscription vẫn có thể Ready (trường hợp nào đó), thì set ValidFrom nếu chưa có
                userTicket.ValidFrom ??= nowUtc;

                _userTicketRepository.Update(userTicket);
            }
            else if (userTicket.Status != UserTicketStatus.Active)
            {
                throw new BadRequestException("Vé không còn sử dụng được.");
            }

            // ===== Tạo Rental =====
            var rental = new Rental
            {
                UserId = userId,
                VehicleId = vehicle.Id,
                StartStationId = startStationId,
                StartTime = nowUtc,
                Status = RentalStatus.Ongoing, 
                CreatedAt = nowUtc
            };

            await _rentalRepo.AddAsync(rental);
            await _uow.SaveChangesAsync(ct); // để có rental.Id

            // ===== BookingTicket gắn vé với chuyến đi =====
            var bookingTicket = new BookingTicket
            {
                UserTicketId = userTicket.Id,
                RentalId = rental.Id,
                AppliedAt = nowUtc,
                VehicleType = vehicle.Category.Name,
                PlanPrice = (decimal)userTicket.PurchasedPrice // hoặc planPrice.Price
            };

            await _bookingTicketRepo.AddAsync(bookingTicket);

            // ===== History Start (lưu UTC, mô tả có thể ghi thêm giờ VN) =====
            var startHistory = new RentalHistory
            {
                RentalId = rental.Id,
                Timestamp = nowUtc,
                ActionType = "Start",
                Description =
                    $"Bắt đầu (giờ VN: {ToVn(nowUtc):HH:mm dd/MM}) tại trạm {startStationId} với xe {vehicle.BikeCode} ({vehicle.Category.Name})."
            };
            await _rentalHistoryRepo.AddAsync(startHistory);

            // ===== Cập nhật xe =====
            vehicle.Status = VehicleStatus.InUse;
            vehicle.StationId = null;
            _vehicleRepo.Update(vehicle);

            await _uow.SaveChangesAsync(ct);

            _logger.LogInformation("Rental {RentalId} created successfully for user {UserId}.", rental.Id, userId);
            return rental.Id;
        }

        //END RENTAL 
        public async Task<bool> EndRentalAsync(EndRentalRequestDTO dto, CancellationToken ct = default)
        {
            var rentalId = dto.RentalId;
            var nowUtc = NowUtc();
            var nowVn = NowVn();

            try
            {
                _logger.LogInformation("Attempting to end rental with ID: {RentalId}", rentalId);

                var rental = await _rentalRepo.Query()
                    .Include(r => r.BookingTickets)
                    .FirstOrDefaultAsync(r => r.Id == rentalId, ct);

                if (rental == null || rental.Status != RentalStatus.Ongoing)
                    throw new NotFoundException("Không tìm thấy cuốc xe hợp lệ hoặc cuốc xe đã kết thúc.");

                // ==== Tìm trạm gần nhất trong bán kính ====
                var stations = await _stationRepo.Query()
                    .Where(s => s.IsActive)
                    .ToListAsync(ct);

                if (!stations.Any())
                    throw new BadRequestException("Không có trạm hoạt động nào khả dụng để trả xe.");

                var nearestStation = stations
                    .Select(s => new
                    {
                        Station = s,
                        Distance = GeolocationHelper.CalculateDistanceInMeters(
                            dto.CurrentLatitude,
                            dto.CurrentLongitude,
                            (double)(s.Lat ?? 0),
                            (double)(s.Lng ?? 0))
                    })
                    .OrderBy(x => x.Distance)
                    .First();

                const double allowedRadius = 10.0;
                if (nearestStation.Distance > allowedRadius)
                {
                    throw new BadRequestException(
                        $"Bạn cần ở trong phạm vi {allowedRadius}m của trạm gần nhất ({nearestStation.Station.Name}). " +
                        $"Khoảng cách hiện tại là {nearestStation.Distance:F2}m.");
                }

                //Cập nhật rental & xe
                rental.EndStationId = nearestStation.Station.Id;
                rental.EndTime = nowUtc;
                rental.Status = RentalStatus.End;
                _rentalRepo.Update(rental);

                var vehicle = await _vehicleRepo.GetByIdAsync(rental.VehicleId);
                if (vehicle != null)
                {
                    vehicle.Status = VehicleStatus.Available;
                    vehicle.StationId = nearestStation.Station.Id;
                    _vehicleRepo.Update(vehicle);
                }
                //Xử lý vé
                var duration = (int)(nowUtc - rental.StartTime).TotalMinutes;
                var bookingTicket = rental.BookingTickets.FirstOrDefault();

                UserTicket? userTicket = null;
                TicketPlanPrice? plan = null;

                int freeFromRemaining = 0; // dùng DurationLimitMinutes / RemainingMinutes
                int freePerRide = 0; // dùng DailyFreeDurationMinutes
                int overtimeMinutes = 0;
                decimal overtimeFee = 0m;

                if (bookingTicket != null)
                {
                    userTicket = await _userTicketRepository.GetByIdAsync(bookingTicket.UserTicketId);
                    if (userTicket != null)
                    {
                        plan = await _planPriceRepo.GetByIdAsync(userTicket.PlanPriceId, ct);

                        var isSubscription = plan?.ValidityDays.GetValueOrDefault() > 0; // vé ngày / vé tháng
                        var isTripPass = !isSubscription;                             // vé lượt

                        // a) Trừ lượt nếu có (vé lượt)
                        if (userTicket.RemainingRides.HasValue && userTicket.RemainingRides.Value > 0)
                        {
                            userTicket.RemainingRides -= 1;
                        }

                        // b) Free theo "tổng phút của vé" (vé lượt + vé ngày)
                        if (plan?.DurationLimitMinutes.HasValue == true &&
                            userTicket.RemainingMinutes.HasValue)
                        {
                            var remainingBefore = userTicket.RemainingMinutes.Value;
                            freeFromRemaining = Math.Min(remainingBefore, duration);
                            userTicket.RemainingMinutes = Math.Max(0, remainingBefore - freeFromRemaining);
                        }

                        // c) Free theo mỗi chuyến (vé tháng 79k/300k)
                        if (plan?.DailyFreeDurationMinutes.HasValue == true)
                        {
                            var rest = duration - freeFromRemaining;
                            if (rest > 0)
                            {
                                freePerRide = Math.Min(rest, plan.DailyFreeDurationMinutes.Value);
                            }
                        }

                        // d) Phút vượt → tạo nợ
                        overtimeMinutes = duration - freeFromRemaining - freePerRide;
                        if (overtimeMinutes < 0) overtimeMinutes = 0;

                        if (overtimeMinutes > 0 && (plan?.OverageFeePer15Min ?? 0) > 0)
                        {
                            var blocks = Math.Ceiling(overtimeMinutes / 15.0); // 1–15p = 1 block
                            overtimeFee = (decimal)blocks * plan!.OverageFeePer15Min!.Value;
                        }

                        // e) Cập nhật trạng thái vé
                        bool exhaustedRides = userTicket.RemainingRides.HasValue && userTicket.RemainingRides.Value <= 0;
                        bool exhaustedMinutes = userTicket.RemainingMinutes.HasValue && userTicket.RemainingMinutes.Value <= 0;
                        var exp = EffectiveExpiry(userTicket);
                        bool expiredByDate = exp.HasValue && exp.Value <= nowUtc;

                        if (expiredByDate)
                        {
                            userTicket.Status = UserTicketStatus.Expired;
                        }
                        else
                        {
                            if (isTripPass)
                            {
                                // Vé lượt: dùng đúng 1 chuyến → sau chuyến đầu là Used
                                userTicket.RemainingRides = 0;
                                userTicket.RemainingMinutes = 0;

                                if (userTicket.Status != UserTicketStatus.Expired)
                                    userTicket.Status = UserTicketStatus.Used;
                            }
                            else
                            {
                                // Subscription (vé ngày / vé tháng)
                                if (exhaustedRides || exhaustedMinutes)
                                    userTicket.Status = UserTicketStatus.Used;
                                else
                                    userTicket.Status = UserTicketStatus.Active;
                            }
                        }

                        _userTicketRepository.Update(userTicket);
                    }

                    bookingTicket.UsedMinutes = duration;
                    bookingTicket.OverusedMinutes = overtimeMinutes > 0 ? overtimeMinutes : null;
                    bookingTicket.OverusedFee = overtimeFee > 0 ? overtimeFee : null;
                    bookingTicket.AppliedAt = nowUtc;
                    _bookingTicketRepo.Update(bookingTicket);
                }

                // ====== Tạo Order + WalletDebt nếu có overtimeFee ======
                if (overtimeFee > 0)
                {
                    var wallet = await _walletRepo.Query()
                        .FirstOrDefaultAsync(w => w.UserId == rental.UserId, ct);

                    var overtimeOrder = new Order
                    {
                        UserId = rental.UserId,
                        OrderNo = GenerateOvertimeOrderNo(rental.UserId),
                        OrderType = OrderTypeConstants.RentalOvertime,
                        Status = OrderStatus.Pending,
                        Subtotal = overtimeFee,
                        Discount = 0,
                        Total = overtimeFee,
                        Currency = wallet?.Currency ?? "VND",
                        CreatedAt = nowUtc
                    };

                    await _orderRepo.AddAsync(overtimeOrder, ct);
                    await _uow.SaveChangesAsync(ct); 

                    var debt = new WalletDebt
                    {
                        UserId = rental.UserId,
                        OrderId = overtimeOrder.Id,
                        Amount = overtimeFee,
                        Remaining = overtimeFee,
                        Status = WalletDebtStatus.Unpaid,
                        CreatedAt = nowUtc
                    };
                    await _walletDebtRepo.AddAsync(debt, ct);

                    if (wallet != null)
                    {
                        wallet.TotalDebt += overtimeFee;
                        wallet.UpdatedAt = nowUtc;
                        _walletRepo.Update(wallet);
                    }

                    _logger.LogInformation(
                        "User {UserId} overtime {OverMinutes} minutes (fee {Fee}) on rental {RentalId}, order {OrderId}.",
                        rental.UserId,
                        overtimeMinutes,
                        overtimeFee,
                        rental.Id,
                        overtimeOrder.Id);
                }

                // ====== RentalHistory End + stats ======
                double? distanceMeters = null;
                double? distanceKmDouble = null;
                double? co2SavedKg = null;
                double? caloriesBurned = null;

                if (dto.DistanceMeters.HasValue && dto.DistanceMeters.Value > 0)
                {
                    distanceMeters = dto.DistanceMeters.Value;
                    distanceKmDouble = distanceMeters.Value / 1000.0;
                    co2SavedKg = distanceKmDouble.Value * 0.21;     
                    caloriesBurned = distanceKmDouble.Value * 35.0; 
                }
                string descriptionBase =
                    $"Kết thúc (giờ VN: {ToVn(nowUtc):HH:mm dd/MM}) tại trạm {nearestStation.Station.Name}";

                string description;

                if (distanceMeters.HasValue)
                {
                    description =
                        $"{descriptionBase}, ~{distanceMeters.Value:F0}m (quãng đường do app ghi nhận), {duration} phút, " +
                        $"Tiêu hao ~{(caloriesBurned ?? 0):F0} kcal, tiết kiệm ~{(co2SavedKg ?? 0):F3} kg CO₂.";
                }
                else
                {
                    description = $"{descriptionBase}, thời gian {duration} phút.";
                }

                var history = new RentalHistory
                {
                    RentalId = rental.Id,
                    Timestamp = nowUtc,
                    ActionType = "End",
                    DurationMinutes = duration,
                    DistanceMeters = distanceMeters,               
                    Co2SavedKg = co2SavedKg,
                    CaloriesBurned = caloriesBurned,
                    Description = description
                };

                await _rentalHistoryRepo.AddAsync(history, ct);

                // Cập nhật thống kê người dùng sau chuyến đi
                decimal distanceKmForStats = 0m;
                decimal co2ForStats = 0m;
                decimal caloriesForStats = 0m;

                if (distanceKmDouble.HasValue)
                {
                    distanceKmForStats = (decimal)distanceKmDouble.Value;
                }
                if (co2SavedKg.HasValue)
                {
                    co2ForStats = (decimal)co2SavedKg.Value;
                }
                if (caloriesBurned.HasValue)
                {
                    caloriesForStats = (decimal)caloriesBurned.Value;
                }

                await _userLifetimeStatsService.UpdateAfterRideAsync(
                    rental.UserId,
                    distanceKmForStats,
                    duration,
                    co2ForStats,
                    caloriesForStats,
                    ct);

                await _questService.ProcessRideAsync(
                    rental.UserId,
                    distanceKmForStats,
                    duration,
                    nowUtc,
                    ct);


                // Lưu tất cả
                await _uow.SaveChangesAsync(ct);

                _logger.LogInformation(
                    "Rental {RentalId} ended successfully at station {StationId}.",
                    rentalId,
                    nearestStation.Station.Id);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while ending rental {RentalId}", dto.RentalId);
                throw;
            }
        }
    

        public async Task<VehicleDetailDTO> GetVehicleByCode(RequestVehicleDTO requestVehicleDTO)
        {

            const double allowedRadius = 20.0;

            try
            {
                // Lấy station Lat/Lng của trạm mà xe đang thuộc về
                var station = await _stationRepo.Query()
                    .Where(s => s.Vehicles.Any(v => v.Id == requestVehicleDTO.VehicleId))
                    .FirstOrDefaultAsync();

                if (station == null || station.Lat == null || station.Lng == null)
                {
                    _logger.LogWarning("Xe ID {BikeCode} không thuộc trạm hợp lệ hoặc trạm thiếu tọa độ.", requestVehicleDTO.VehicleId);
                    throw new NotFoundException("Không tìm thấy trạm xe hợp lệ cho mã xe này.");
                }

                // Tính khoảng cách người dùng đến trạm
                double distance = GeolocationHelper.CalculateDistanceInMeters(
                    requestVehicleDTO.CurrentLatitude,
                    requestVehicleDTO.CurrentLongitude,
                    (double)station.Lat.Value,
                    (double)station.Lng.Value
                );

                // So sánh khoảng cách với bán kính cho phép
                if (distance > allowedRadius)
                {
                    _logger.LogWarning("User ID {UserId} cố gắng quét QR code từ xa. Khoảng cách: {Distance:F2}m.",
                        _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier), distance);

                    throw new BadRequestException($"Bạn cần ở trong phạm vi {allowedRadius}m của trạm xe ({station.Name}) để quét QR. Khoảng cách hiện tại là {distance:F2}m.");
                }

                // Nếu kiểm tra vị trí thành công: Lấy thông tin chi tiết của xe
                var vehicle = await _vehicleRepo.GetVehicleWithCategoryAsync(requestVehicleDTO.VehicleId);

                if (vehicle == null)
                {
                    // Dù đã tìm thấy trạm, vẫn cần kiểm tra xe có sẵn sàng không
                    _logger.LogWarning("Không tìm thấy thông tin chi tiết xe ID {BikeCode} hoặc xe không ở trạng thái sẵn sàng.", requestVehicleDTO.VehicleId);
                    throw new NotFoundException("Thông tin xe không khả dụng.");
                }

                var vehicleDetailDTO = new VehicleDetailDTO
                {
                    BikeCode = vehicle.BikeCode,
                    CategoryName = vehicle.Category?.Name,
                    StationName = vehicle.Station.Name.ToString(),
                    VehicleStatus = vehicle.Status,
                };

                // Trả về thông tin xe (VehicleDetailDTO)
                return vehicleDetailDTO; // Trả về DTO chứa thông tin chi tiết xe
            }
            catch (Exception ex) when (ex is NotFoundException || ex is BadRequestException)
            {
                throw;
            }
            catch (Exception ex)
            {
                // Chú ý: Bạn đã thay đổi tên DTO trong log error (vehicleDetailDTO -> requestVehicleDTO)
                _logger.LogError(ex, "Error in GetVehicleByCode with BikeCode: {BikeCode}", requestVehicleDTO.VehicleId);
                throw new Exception("Lỗi hệ thống khi xử lý yêu cầu.");
            }
        }
        public async Task<IReadOnlyList<RentalHistoryDTO>> GetMyRentalHistoryAsync(CancellationToken ct = default)
        {
            var userId = GetCurrentUserIdOrThrow();

            var rentals = await _rentalRepo.Query()
                .Where(r => r.UserId == userId && r.Status == RentalStatus.End)
                .Include(r => r.Vehicle).ThenInclude(v => v.Category)
                .Include(r => r.BookingTickets)
                    .ThenInclude(bt => bt.UserTicket)
                        .ThenInclude(ut => ut.PlanPrice)
                            .ThenInclude(pp => pp.Plan)
                .Include(r => r.Histories) 
                .AsNoTracking()
                .OrderByDescending(r => r.StartTime)
                .ToListAsync(ct);

            var result = new List<RentalHistoryDTO>();

            foreach (var r in rentals)
            {
                var booking = r.BookingTickets
                    .OrderByDescending(bt => bt.AppliedAt)
                    .FirstOrDefault();
                var endHistory = r.Histories?
                    .FirstOrDefault(h => h.ActionType == "End");

                // Lấy thông tin trạm từ rental (vì đã được lưu trực tiếp)
                var startStation = await _stationRepo.GetByIdAsync(r.StartStationId);
                var endStation = r.EndStationId.HasValue ?
                    await _stationRepo.GetByIdAsync(r.EndStationId.Value) : null;

                int? durationMinutes = null;
                if (r.EndTime.HasValue)
                {
                    durationMinutes = (int)(r.EndTime.Value - r.StartTime).TotalMinutes;
                }

                var dto = new RentalHistoryDTO
                {
                    RentalId = r.Id,
                    StartTimeUtc = r.StartTime,
                    EndTimeUtc = r.EndTime,
                    StartTimeVn = ToVn(r.StartTime),
                    EndTimeVn = r.EndTime.HasValue ? ToVn(r.EndTime.Value) : (DateTimeOffset?)null,

                    StartStationName = startStation?.Name,
                    EndStationName = endStation?.Name,

                    VehicleCode = r.Vehicle?.BikeCode,
                    VehicleType = r.Vehicle?.Category?.Name,

                    UserTicketId = booking?.UserTicketId,
                    TicketPlanName = booking?.UserTicket?.PlanPrice?.Plan?.Name,
                    TicketType = booking?.UserTicket?.PlanPrice?.Plan?.Type,
                    TicketVehicleType = booking?.UserTicket?.PlanPrice?.VehicleType,

                    DurationMinutes = durationMinutes,
                    DistanceKm = endHistory?.DistanceMeters / 1000.0,
                    Co2SavedKg = (decimal?)endHistory?.Co2SavedKg,
                    CaloriesBurned = (decimal?)endHistory?.CaloriesBurned,

                    OverusedMinutes = booking?.OverusedMinutes,
                    OverusedFee = booking?.OverusedFee,

                    Status = r.Status
                };

                result.Add(dto);
            }

            return result;
        }
    }
}
