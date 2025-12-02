using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces.Base;

namespace Application.Interfaces.Staff.Repository
{
    public interface IWalletTransactionRepository : IRepository<Domain.Entities.WalletTransaction, long>
    { }
    
}
