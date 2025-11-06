using Application.DTOs.Rental;
using Application.Exceptions;
using Application.Interfaces;
using Application.Interfaces.Staff.Repository;
using Application.Interfaces.User.Repository;
using Application.Interfaces.User.Service;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Http;
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
        private readonly IRentalsRepository _rentalRepo;
        private readonly IBookingTicketRepository _bookingTicketRepo;
        private readonly IUnitOfWork _uow;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<RentalsService> _logger;

        public RentalsService(
            IStationsRepository stationRepo,
            IVehicleRepository vehicleRepo,
            IRentalsRepository rentalRepo,
            IBookingTicketRepository bookingTicketRepo,
            IUnitOfWork uow,
            IHttpContextAccessor httpContextAccessor,
            ILogger<RentalsService> logger)
        {
            _stationRepo = stationRepo;
            _vehicleRepo = vehicleRepo;
            _rentalRepo = rentalRepo;
            _bookingTicketRepo = bookingTicketRepo;
            _uow = uow;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public async Task<bool> CreateRentalAsync(CreateRentalDTO createRentalDTO)
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

            // Cập nhật trạng thái xe (đang được EF tracking)
            vehicle.Status = VehicleStatus.InUse;
            vehicle.StationId = null;

            await _uow.SaveChangesAsync();

            _logger.LogInformation("Rental {RentalId} created successfully for user {UserId}.", rental.Id, userId);
            return true;
        }

        public Task<bool> EndRentalAsync(EndRentalRequestDTO endRentalDto)
        {
            throw new NotImplementedException();
        }
    }
}
