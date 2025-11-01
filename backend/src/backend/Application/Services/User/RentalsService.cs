using Application.DTOs;
using Application.DTOs.Rental;
using Application.Exceptions;
using Application.Helpers;
using Application.Interfaces;
using Application.Interfaces.Base;
using Application.Interfaces.User.Service;
using Application.Services.Base;
using AutoMapper;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging; // ✅ 1. THÊM USING CHO LOGGER
using System;
using System.Threading.Tasks;

namespace Application.Services.User
{
    public class RentalsService : GenericService<Rental, RentalDTO, long>, IRentalsService
    {
        private readonly IRepository<Station, long> _stationRepo;
        private readonly IRepository<Vehicle, long> _vehicleRepo;
        private readonly ILogger<RentalsService> _logger; 

        public RentalsService(
            IRepository<Rental, long> repo,
            IMapper mapper,
            IUnitOfWork uow,
            IRepository<Station, long> stationRepo,
            IRepository<Vehicle, long> vehicleRepo,
            ILogger<RentalsService> logger) 
            : base(repo, mapper, uow)
        {
            _stationRepo = stationRepo;
            _vehicleRepo = vehicleRepo;
            _logger = logger;
        }

        public async Task EndRentalAsync(long rentalId, EndRentalRequestDTO endRentalDto)
        {
            try
            {
                _logger.LogInformation("Attempting to end rental with ID: {RentalId}", rentalId);

                var rental = await _repo.Query().FirstOrDefaultAsync(r => r.Id == rentalId);
                if (rental == null || rental.Status != "Ongoing")
                {
                    throw new NotFoundException($"Không tìm thấy cuốc xe hợp lệ với ID {rentalId} hoặc cuốc xe đã kết thúc.");
                }

                var endStation = await _stationRepo.GetByIdAsync(endRentalDto.EndStationId);
                if (endStation == null || !endStation.IsActive)
                {
                    throw new BadRequestException("Trạm trả xe không hợp lệ hoặc không hoạt động.");
                }

                const double allowedRadiusInMeters = 3.0;
                double stationLat = (double)(endStation.Lat ?? 0.0m);
                double stationLng = (double)(endStation.Lng ?? 0.0m);

                var distance = GeolocationHelper.CalculateDistanceInMeters(
                    endRentalDto.CurrentLatitude,
                    endRentalDto.CurrentLongitude,
                    stationLat,
                    stationLng
                );

                if (distance > allowedRadiusInMeters)
                {
                    _logger.LogWarning("User {UserId} failed to end rental {RentalId} due to being out of range. Distance: {Distance}m", rental.UserId, rentalId, distance);
                    throw new BadRequestException($"Bạn phải ở trong phạm vi {allowedRadiusInMeters}m của trạm để trả xe. Khoảng cách hiện tại của bạn là {distance:F2}m.");
                }

                var now = DateTimeOffset.UtcNow;
                rental.EndStationId = endRentalDto.EndStationId;
                rental.Status = "Ended";
                rental.EndTime = now;
                _repo.Update(rental);

                var vehicle = await _vehicleRepo.GetByIdAsync(rental.VehicleId);
                if (vehicle != null)
                {
                    vehicle.Status = "Available";
                    vehicle.StationId = endRentalDto.EndStationId;
                    _vehicleRepo.Update(vehicle);
                }

                await _uow.SaveChangesAsync();

                _logger.LogInformation("Successfully ended rental with ID: {RentalId}", rentalId);
            }
            catch (Exception ex)
            {
                // ✅ GHI LOG LỖI CHI TIẾT VÀ NÉM LẠI ĐỂ MIDDLEWARE BẮT
                _logger.LogError(ex, "An unexpected error occurred while ending rental {RentalId}", rentalId);
                throw;
            }
        }
    }
}

