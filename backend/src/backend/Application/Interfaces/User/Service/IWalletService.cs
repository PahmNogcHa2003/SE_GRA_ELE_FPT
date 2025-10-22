using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.User.Service
{
    public interface IWalletService
    {
        Task CreditAsync(long userId, decimal amount, string source, long? orderId, CancellationToken cancellationToken);
        Task DebitAsync(long userId, decimal amount, string reason, long? orderId, CancellationToken cancellationToken);
    }
}
