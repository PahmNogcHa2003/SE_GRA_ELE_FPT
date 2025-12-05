using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces.User.Repository;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.User
{
    public class WalletDebtRepository : BaseRepository<WalletDebt,long>, IWalletDebtRepository
    {
        public WalletDebtRepository(HolaBikeContext context) : base(context)
        {
        }
        public async Task<List<WalletDebt>> GetUnpaidDebtsByUserIdAsync (long userId, CancellationToken cancellationToken)
        {
            return await _dbContext.WalletDebts
                 .Where(d => d.UserId == userId && d.Status == WalletDebtStatus.Unpaid)
                 .OrderBy(d => d.CreatedAt)
                 .ToListAsync(cancellationToken);
        }
        public async Task<bool> HasUnpaidDebtsAsync(long userId, CancellationToken ct)
        {
            return await _dbSet.AnyAsync(d => d.UserId == userId && d.Remaining > 0 && d.Status == "Unpaid", ct);
        }

    }
}
