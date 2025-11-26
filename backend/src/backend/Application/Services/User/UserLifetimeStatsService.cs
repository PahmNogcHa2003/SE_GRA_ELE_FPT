using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.RentalHistory;
using Application.Interfaces;
using Application.Interfaces.Base;
using Application.Interfaces.User.Service;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.User
{
    public class UserLifetimeStatsService : IUserLifetimeStatsService
    {
        private readonly IRepository<UserLifetimeStats, long> _userLifetimeStatsRepo;
        private readonly IUnitOfWork _unitOfWork;
        public UserLifetimeStatsService(
            IRepository<UserLifetimeStats, long> userLifetimeStatsRepo,
            IUnitOfWork unitOfWork)
        {
            _userLifetimeStatsRepo = userLifetimeStatsRepo;
            _unitOfWork = unitOfWork;
        }
        public async Task UpdateAfterRideAsync(
            long userId, 
            decimal distanceKm, 
            int durationMinutes, 
            decimal co2SavedKg, 
            decimal caloriesBurned, 
            CancellationToken ct = default)
        {
            if(distanceKm < 0 || durationMinutes < 0 || co2SavedKg <0 || caloriesBurned < 0)
            {
                throw new ArgumentException("Ride statistics values must be non-negative.");
            }
             var userLifetimeStats = await _userLifetimeStatsRepo
                .Query()
                .FirstOrDefaultAsync(uls => uls.UserId == userId, ct);
            if (userLifetimeStats == null)
            {
                userLifetimeStats = new UserLifetimeStats
                {
                    UserId = userId,
                    TotalDistanceKm = distanceKm,
                    TotalDurationMinutes = durationMinutes,
                    TotalCo2SavedKg = co2SavedKg,
                    TotalCaloriesBurned = caloriesBurned,
                    TotalTrips = 1,
                    UpdatedAt = DateTimeOffset.UtcNow
                };
                await _userLifetimeStatsRepo.AddAsync(userLifetimeStats, ct);
            }
            else
            {
                userLifetimeStats.TotalDistanceKm += distanceKm;
                userLifetimeStats.TotalDurationMinutes += durationMinutes;
                userLifetimeStats.TotalCo2SavedKg += co2SavedKg;
                userLifetimeStats.TotalCaloriesBurned += caloriesBurned;
                userLifetimeStats.TotalTrips += 1;
                userLifetimeStats.UpdatedAt = DateTimeOffset.UtcNow;
                _userLifetimeStatsRepo.Update(userLifetimeStats);
            }
            await _unitOfWork.SaveChangesAsync(ct);
        }
        public async Task<RentalStatsSummaryDTO> GetMyStatsAsync(long userId, CancellationToken ct = default)
        {
            var userLifetimeStats = await _userLifetimeStatsRepo
                .Query()
                .FirstOrDefaultAsync(uls => uls.UserId == userId, ct);
            if (userLifetimeStats == null)
            {
                return new RentalStatsSummaryDTO
                {
                    TotalDistanceKm = 0,
                    TotalDurationMinutes = 0,
                    TotalCo2SavedKg = 0,
                    TotalCaloriesBurned = 0,
                    TotalTrips = 0
                };
            }
            return new RentalStatsSummaryDTO
            {
                TotalDistanceKm = userLifetimeStats.TotalDistanceKm,
                TotalDurationMinutes = userLifetimeStats.TotalDurationMinutes,
                TotalCo2SavedKg = userLifetimeStats.TotalCo2SavedKg,
                TotalCaloriesBurned = userLifetimeStats.TotalCaloriesBurned,
                TotalTrips = userLifetimeStats.TotalTrips
            };
        }
    }
}
