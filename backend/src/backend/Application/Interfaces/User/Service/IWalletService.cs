using Application.DTOs.Wallet;
using Application.Interfaces.Base;
using Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Interfaces.User.Service
{
   public interface IWalletService : IService<Domain.Entities.Wallet, WalletDTO, long>
   {
        Task<WalletTransaction> CreditAsync(long userId, decimal amount, string source, long? orderId, CancellationToken cancellationToken);
        Task<WalletTransaction> DebitAsync(long userId, decimal amount, string reason, long? orderId, CancellationToken cancellationToken);
        Task<WalletTransaction> CreditPromoAsync(long userId, decimal amount, string source, CancellationToken cancellationToken);
        Task<WalletTransaction> ConvertPromoToBalanceAsync(long? userId, decimal amount, CancellationToken cancellationToken);
        Task<PayDebtResultDTO> PayAllDebtFromBalanceAsync(long userId, CancellationToken ct);

    }
}



