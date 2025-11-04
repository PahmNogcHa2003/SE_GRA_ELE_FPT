using Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Interfaces.User.Service
    {
        public interface IWalletService
        {

            Task<WalletTransaction> CreditAsync(long userId, decimal amount, string source, long? orderId, CancellationToken cancellationToken);
            Task<WalletTransaction> DebitAsync(long userId, decimal amount, string reason, long? orderId, CancellationToken cancellationToken);
        }
   }



