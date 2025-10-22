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
    public class WalletRepository : BaseRepository<Wallet, long>, IWalletRepository
    {
        public WalletRepository(HolaBikeContext context) : base(context)
        {
        }

        public async Task<Wallet?> GetByUserIdAsync(long userId, CancellationToken cancellationToken)
        {

            return await _dbContext.Wallets.FirstOrDefaultAsync(w => w.UserId == userId, cancellationToken);
        }
    }
}
