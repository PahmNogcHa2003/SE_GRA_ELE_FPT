using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces.User.Repository;
using Domain.Entities;
using Infrastructure.Persistence;

namespace Infrastructure.Repositories.User
{
    public class WalletTransactionRepository : BaseRepository<WalletTransaction, long>, IWalletTransactionRepository
    {
        public WalletTransactionRepository(HolaBikeContext context) : base(context)
        {
        }
 
    }
}
