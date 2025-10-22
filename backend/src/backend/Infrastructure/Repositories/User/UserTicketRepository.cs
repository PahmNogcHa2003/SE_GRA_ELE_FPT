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
    public class UserTicketRepository : BaseRepository<Domain.Entities.UserTicket, long>, IUserTicketRepository
    {
        public UserTicketRepository(HolaBikeContext context) : base(context)
        {
        }
        public async Task<bool> HasExistingSubscriptionAsync(long userId, long planPriceId, CancellationToken ct = default)
        {
            return await _dbSet
                .AnyAsync(ut =>
                    ut.UserId == userId &&
                    ut.PlanPriceId == planPriceId &&
                    (ut.Status == "Active" || ut.Status == "Ready"), ct);
        }

        public async Task<List<UserTicket>> GetActiveTicketsByUserIdAsync(long userId, CancellationToken ct = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(ut => ut.PlanPrice)      
                    .ThenInclude(pp => pp.Plan) 
                .Where(ut => ut.UserId == userId && ut.Status != "Expired")
                .OrderByDescending(ut => ut.CreatedAt)
                .ToListAsync(ct);
        }
    }
}
