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
using System.Security.Claims;
using System.Threading.Tasks;

namespace Application.Services.User
{
    public class RentalsService : IRentalsService
    {
        private readonly IStationsRepository _stationRepo;
        private readonly IVehicleRepository _vehicleRepo;
        private readonly IUserTicketRepository _userTicketRepository;
        private readonly IRentalsRepository _rentalRepo;
        private readonly IBookingTicketRepository _bookingTicketRepo;
        private readonly IUnitOfWork _uow;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<RentalsService> _logger;
        private readonly IWalletDebtRepository _walletDebtRepository;

        public RentalsService(
            IStationsRepository stationRepo,
            IVehicleRepository vehicleRepo,
            IUserTicketRepository userTicketRepository,
            IRentalsRepository rentalRepo,
            IBookingTicketRepository bookingTicketRepo,
            IUnitOfWork uow,
            IHttpContextAccessor httpContextAccessor,
            ILogger<RentalsService> logger,
            IWalletDebtRepository walletDebtRepository)
        {
            _stationRepo = stationRepo;
            _vehicleRepo = vehicleRepo;
            _userTicketRepository = userTicketRepository;
            _rentalRepo = rentalRepo;
            _bookingTicketRepo = bookingTicketRepo;
            _uow = uow;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _walletDebtRepository = walletDebtRepository;
        }

        public async Task<long> CreateRentalAsync(CreateRentalDTO createRentalDTO)
        {
            // Lấy stationId từ vehicle
            var startStationId = await _vehicleRepo.GetStationVehicle(createRentalDTO.VehicleId);
            if (startStationId == 0)
            {
                _logger.LogWarning("Xe ID {VehicleId} không tồn tại hoặc không ở trạm nào.", createRentalDTO.VehicleId);
                throw new NotFoundException("Xe không tồn tại hoặc không ở trạm nào.");
            }

            // Lấy userId từ Claims
            var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim) || !long.TryParse(userIdClaim, out long userId))
            {
                _logger.LogWarning("Không tìm thấy User ID trong Claims.");
                throw new UnauthorizedAccessException("User chưa đăng nhập.");
            }

            // Kiểm tra Vehicle có tồn tại không
            var vehicle = await _vehicleRepo.GetVehicleWithCategoryAsync(createRentalDTO.VehicleId);
            if (vehicle == null)
            {
                _logger.LogWarning("Không tìm thấy thông tin loại xe cho Vehicle ID {VehicleId}.", createRentalDTO.VehicleId);
                throw new NotFoundException("Không tìm thấy loại xe.");
            }

            if (vehicle.Status == VehicleStatus.InUse)
            {
                throw new BadRequestException("Xe này đang được sử dụng.");
            }

            // Tạo record Rental
            var rental = new Rental
            {
                UserId = userId,
                VehicleId = vehicle.Id,
                StartStationId = startStationId,
                StartTime = DateTimeOffset.UtcNow,
                Status = "Ongoing",
                CreatedAt = DateTimeOffset.UtcNow
            };

            await _rentalRepo.AddAsync(rental);
            await _uow.SaveChangesAsync();


            // Tạo Booking Ticket (gắn vé cho lượt thuê)
            var bookingTicket = new BookingTicket
            {
                UserTicketId = createRentalDTO.UserTicketId,
                RentalId = rental.Id,
                AppliedAt = DateTimeOffset.UtcNow,
                VehicleType = vehicle.Category.Name,
                PlanPrice = 0
            };

            await _bookingTicketRepo.AddAsync(bookingTicket);
            await _uow.SaveChangesAsync();


            // Cập nhật trạng thái xe (đang được EF tracking)
            vehicle.Status = VehicleStatus.InUse;
            vehicle.StationId = null;

            await _uow.SaveChangesAsync();

            _logger.LogInformation("Rental {RentalId} created successfully for user {UserId}.", rental.Id, userId);
            return rental.Id;
        }

        public async Task<bool> EndRentalAsync(EndRentalRequestDTO endRentalDto)
        {
            long rentalId = endRentalDto.RentalId;
            try
            {
                
                _logger.LogInformation("Attempting to end rental with ID: {RentalId}", rentalId);

                // 1️⃣ Kiểm tra rental
                var rental = await _rentalRepo.Query().Include(r => r.BookingTickets).FirstOrDefaultAsync(r => r.Id == rentalId);
                if (rental == null || rental.Status != "Ongoing")
                    throw new NotFoundException($"Không tìm thấy cuốc xe hợp lệ với ID {rentalId} hoặc cuốc xe đã kết thúc.");

                // 2️⃣ Lấy danh sách trạm đang hoạt động
                var stations = await _stationRepo.Query().Where(s => s.IsActive).ToListAsync();
                if (!stations.Any())
                    throw new BadRequestException("Không có trạm hoạt động nào khả dụng để trả xe.");

                // 3️⃣ Tìm trạm gần nhất
                var nearestStation = stations
                    .Select(s => new
                    {
                        Station = s,
                        Distance = GeolocationHelper.CalculateDistanceInMeters(
                            endRentalDto.CurrentLatitude,
                            endRentalDto.CurrentLongitude,
                            (double)(s.Lat ?? 0),
                            (double)(s.Lng ?? 0)
                        )
                    })
                    .OrderBy(x => x.Distance)
                    .First();

                const double allowedRadius = 4.0;
                if (nearestStation.Distance > allowedRadius)
                {
                    throw new BadRequestException($"Bạn cần ở trong phạm vi {allowedRadius}m của trạm gần nhất ({nearestStation.Station.Name}). Khoảng cách hiện tại là {nearestStation.Distance:F2}m.");
                }

                var now = DateTimeOffset.UtcNow;

                // 4️⃣ Cập nhật thông tin chuyến đi
                rental.EndStationId = nearestStation.Station.Id;
                rental.EndTime = now;
                rental.Status = RentalStatus.End;
                _rentalRepo.Update(rental);

                // 5️⃣ Cập nhật trạng thái xe
                var vehicle = await _vehicleRepo.GetByIdAsync(rental.VehicleId);
                if (vehicle != null)
                {
                    vehicle.Status = "Available";
                    vehicle.StationId = nearestStation.Station.Id;
                    _vehicleRepo.Update(vehicle);
                }

                // 6️⃣ Cập nhật vé người dùng (UserTicket)
                var userTicket = await _userTicketRepository.Query()
                    .FirstOrDefaultAsync(ut => ut.UserId == rental.UserId && ut.Status == "Active");

                // Thời gian đi
                var duration = (int)(now - rental.StartTime).TotalMinutes;
                if (userTicket != null)
                {
                    if (userTicket.RemainingRides.HasValue && userTicket.RemainingRides > 0)
                        userTicket.RemainingRides -= 1;

                    if (userTicket.RemainingMinutes.HasValue)
                    {
                        userTicket.RemainingMinutes = Math.Max(0, userTicket.RemainingMinutes.Value - duration);
                    }

                    // Nếu hết vé => đổi trạng thái
                    if ((userTicket.RemainingRides <= 0 && userTicket.RemainingRides != null)
                        || (userTicket.RemainingMinutes <= 0 && userTicket.RemainingMinutes != null))
                    {
                        userTicket.Status = "Expired";
                    }

                    _userTicketRepository.Update(userTicket);
                }

                // Kiểm tra tiền nợ (nếu có)
                if (duration > userTicket.RemainingMinutes)
                {
                    // Tính tiền nợ và xử lý (nếu có)
                    var overageMinutes = duration - (userTicket.RemainingMinutes ?? 0);
                    var overageFeePerMinute = 3000; // Giả sử 3000 VND/phút
                    var totalOverageFee = overageMinutes * overageFeePerMinute;
                    // Tạo wallet debt
                    var walletDeb = new WalletDebt
                    {
                        UserId = rental.UserId,
                        Amount = totalOverageFee,
                        Remaining = overageMinutes,
                        Status = WalletDebStatus.Unpaid,
                        CreatedAt = DateTimeOffset.UtcNow,
                    };
                    await _walletDebtRepository.AddAsync(walletDeb); 
                }

                // 7️⃣ Cập nhật booking ticket (nếu có)
                var bookingTicket = rental.BookingTickets.FirstOrDefault();
                if (bookingTicket != null)
                {
                    bookingTicket.UsedMinutes = duration;
                    bookingTicket.AppliedAt = DateTime.Now;
                    _bookingTicketRepo.Update(bookingTicket);
                }

                await _uow.SaveChangesAsync();

                _logger.LogInformation("Rental {RentalId} ended successfully at station {StationId}", rentalId, nearestStation.Station.Id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while ending rental {RentalId}", rentalId);
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
    }
}
