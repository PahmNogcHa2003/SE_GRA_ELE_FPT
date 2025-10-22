using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces.Base;
using Domain.Entities;

namespace Application.Interfaces.User.Repository
{
    public interface IWalletRepository : IRepository<Wallet,long>
    {
        Task<Wallet?> GetByUserIdAsync(long userId, CancellationToken cancellationToken);
    }
}
