using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.RentalHistory;

namespace Application.Interfaces.User.Service
{
    public interface IUserLifetimeStatsService
    {
        Task UpdateAfterRideAsync(long userId,decimal distanceKm,int durationMinutes,decimal co2SavedKg,decimal caloriesBurned,CancellationToken ct = default);

        Task<RentalStatsSummaryDTO> GetMyStatsAsync(long userId, CancellationToken ct = default);
    }
}
