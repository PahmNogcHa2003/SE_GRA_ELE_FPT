using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces.Staff.Repository;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.Staff
{
    public class PromotionCampaignRepository : BaseRepository<Domain.Entities.PromotionCampaign, long>, IPromotionCampaignRepository
    {
        private readonly HolaBikeContext _dbContext;
        public PromotionCampaignRepository(HolaBikeContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<PromotionCampaign?> GetActivePromotionAsync (decimal amount , DateTimeOffset now , CancellationToken ct)
        {
            return await _dbSet
                .Where(pc => pc.Status == "Active"
                             && pc.MinAmount <= amount
                             && pc.StartAt <= now
                             && pc.EndAt >= now)
                .OrderByDescending(pc => pc.BonusPercent)
                .FirstOrDefaultAsync(ct);
        }
    }
}
