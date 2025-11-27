using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces.User.Repository;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.User
{
    public class RentalHistoryRepository : BaseRepository<RentalHistory, long>, IRentalHistoryRepository
    {
        private readonly HolaBikeContext _dbContext;
        public RentalHistoryRepository(HolaBikeContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<List<LeaderboardRawItem>> GetLeaderboardRawAsync(
           DateTimeOffset? fromUtc,
           DateTimeOffset? toUtc,
           int topN,
           CancellationToken ct = default)
        {
           if(topN <= 0) topN = 10;
           var query = _dbContext.RentalHistories
                .Include(rh => rh.Rental)
                .ThenInclude(r => r.User.UserProfile)
                .AsNoTracking()
                .Where(rh => rh.ActionType == "End");
            if (fromUtc.HasValue)
                query = query.Where(rh => rh.Timestamp >= fromUtc.Value);
            if (toUtc.HasValue)
                query = query.Where(rh => rh.Timestamp <= toUtc.Value);
            var leaderboard = await  query
                .GroupBy(rh => new
                {
                    rh.Rental.UserId,
                    rh.Rental.User.UserProfile.FullName,
                    rh.Rental.User.UserProfile.AvatarUrl
                })
                .Select(g => new LeaderboardRawItem
                {
                    UserId = g.Key.UserId,
                    FullName = g.Key.FullName,
                    AvatarUrl = g.Key.AvatarUrl,
                    TotalDurationMinutes = g.Sum(rh => (int?)(rh.DurationMinutes ?? 0)) ?? 0,
                    TotalDistanceKm = g.Sum(rh => (decimal?)(rh.DistanceMeters ?? 0) / 1000) ?? 0,
                    TotalTrips = g.Count()
                })
                .OrderByDescending(item => item.TotalDistanceKm)
                .ThenBy(x => x.UserId)
                .Take(topN)
                .ToListAsync(ct);
            return leaderboard;
        }
    }
}
