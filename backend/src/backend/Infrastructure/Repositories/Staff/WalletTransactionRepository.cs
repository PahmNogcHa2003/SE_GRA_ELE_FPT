using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces.Base;
using Application.Interfaces.Staff.Repository;
using Domain.Entities;
using Infrastructure.Persistence;

namespace Infrastructure.Repositories.Staff
{
    public class WalletTransactionRepository : BaseRepository<WalletTransaction, long>, IWalletTransactionRepository
    {
        public WalletTransactionRepository(HolaBikeContext holaBike) : base(holaBike)
        {

        }
    }
}
