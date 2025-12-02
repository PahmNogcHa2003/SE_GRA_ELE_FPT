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

        /// <summary>
        /// Lấy bảng xếp hạng theo period:
        /// - "week": 7 ngày gần nhất
        /// - "month": 30 ngày gần nhất
        /// - "lifetime": toàn bộ lịch sử (dùng UserLifetimeStats)
        /// </summary>
        Task<IReadOnlyList<LeaderboardEntryDTO>> GetLeaderboardAsync(
            string period,
            int topN,
            CancellationToken ct = default);
    }

}
