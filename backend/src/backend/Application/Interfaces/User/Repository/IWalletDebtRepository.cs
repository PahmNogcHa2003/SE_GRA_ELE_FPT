using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces.Base;
using Domain.Entities;

namespace Application.Interfaces.User.Repository
{
    public interface IWalletDebtRepository : IRepository<WalletDebt,long>
    {
        Task<List<WalletDebt>> GetUnpaidDebtsByUserIdAsync(long userId, CancellationToken cancellationToken);
        Task<bool> HasUnpaidDebtsAsync(long userId, CancellationToken ct);

    }
}
