using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.RentalHistory;
using Application.Interfaces;
using Application.Interfaces.Base;
using Application.Interfaces.User.Repository;
using Application.Interfaces.User.Service;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.User
{
    public class UserLifetimeStatsService : IUserLifetimeStatsService
    {
        private readonly IRepository<UserLifetimeStats, long> _userLifetimeStatsRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRentalHistoryRepository _rentalHistoryRepo;
        private static readonly TimeZoneInfo VnTz =
            TimeZoneInfo.FindSystemTimeZoneById(
                RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                    ? "SE Asia Standard Time"
                    : "Asia/Ho_Chi_Minh");
        public UserLifetimeStatsService(
            IRepository<UserLifetimeStats, long> userLifetimeStatsRepo,
            IRentalHistoryRepository rentalHistoryRepo,
            IUnitOfWork unitOfWork)
        {
            _userLifetimeStatsRepo = userLifetimeStatsRepo;
            _unitOfWork = unitOfWork;
            _rentalHistoryRepo = rentalHistoryRepo;
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
        private static (DateTimeOffset? fromUtc, DateTimeOffset? toUtc) GetPeriodRangeUtc(string period)
        {
            var nowUtc = DateTimeOffset.UtcNow;
            if(string.Equals(period,"week", StringComparison.OrdinalIgnoreCase))
            {
                var nowVn = TimeZoneInfo.ConvertTime(nowUtc, VnTz);
                var fromVn = nowVn.Date.AddDays(-6);
                var fromUtc = TimeZoneInfo.ConvertTimeToUtc(fromVn, VnTz);
                return (fromUtc, nowUtc);
            }
            if(string.Equals(period, "month", StringComparison.OrdinalIgnoreCase))
            {
                var nowVn = TimeZoneInfo.ConvertTime(nowUtc, VnTz);
                var fromVn = nowVn.Date.AddDays(-29);
                var fromUtc = TimeZoneInfo.ConvertTimeToUtc(fromVn, VnTz);
                return (fromUtc, nowUtc);
            }
            return (null, null);
        }
        public async Task<IReadOnlyList<LeaderboardEntryDTO>> GetLeaderboardAsync(
            string period,
            int topN,
            CancellationToken ct = default)
        {
            if(topN <= 0) topN = 10;
            if(string.Equals(period, "lifetime", StringComparison.OrdinalIgnoreCase))
            {
               var data = await _userLifetimeStatsRepo.Query()
                    .Include(s => s.User.UserProfile)
                    .AsNoTracking()
                    .OrderByDescending(s => s.TotalDistanceKm)
                    .Take(topN)
                    .Select(s => new LeaderboardEntryDTO
                    {
                        UserId = s.UserId,
                        FullName = s.User.UserProfile.FullName,
                        AvatarUrl = s.User.UserProfile.AvatarUrl,
                        TotalDurationMinutes = s.TotalDurationMinutes,
                        TotalDistanceKm = s.TotalDistanceKm,
                        TotalTrips = s.TotalTrips,
                        Rank = 0
                    })
                    .ToListAsync(ct);
                for (int i = 0; i < data.Count; i++)
                {
                    data[i].Rank = i + 1;
                }
                return data;
            }
            var (fromUtc, toUtc) = GetPeriodRangeUtc(period);
            var raw = await _rentalHistoryRepo.GetLeaderboardRawAsync(fromUtc, toUtc, topN, ct);

            var result = raw
                .Select((item, index) => new LeaderboardEntryDTO
                {
                    UserId = item.UserId,
                    FullName = item.FullName,
                    AvatarUrl = item.AvatarUrl,
                    TotalDurationMinutes = item.TotalDurationMinutes,
                    TotalDistanceKm = item.TotalDistanceKm,
                    TotalTrips = item.TotalTrips,
                    Rank = index + 1
                })
                .ToList();
            return result;

        }
    }
}
