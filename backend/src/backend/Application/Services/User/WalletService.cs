using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces;
using Application.Interfaces.User.Repository;
using Application.Interfaces.User.Service;
using Domain.Entities;

namespace Application.Services.User
{
    public class WalletService : IWalletService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWalletDebtRepository _walletDebtRepository;
        private readonly IWalletRepository _walletRepository;
        private readonly IWalletTransactionRepository _walletTransactionRepository;
        public WalletService(IUnitOfWork unitOfWork, 
            IWalletDebtRepository walletDebtRepository,
            IWalletRepository walletRepository,
            IWalletTransactionRepository walletTransactionRepository)
        {
            _unitOfWork = unitOfWork;
            _walletDebtRepository = walletDebtRepository;
            _walletRepository = walletRepository;
            _walletTransactionRepository = walletTransactionRepository;
        }
        public async Task CreditAsync(long userId, decimal amount, string source, long? orderId, CancellationToken cancellationToken)
        {
            var wallet = await _walletRepository.GetByUserIdAsync(userId, cancellationToken);

            if (wallet == null)
            {
                wallet = new Wallet { UserId = userId, Status = "Active" };
                await _walletRepository.AddAsync(wallet, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }

            decimal remainingAmount = amount;

            var unpaidDebts = await _walletDebtRepository.GetUnpaidDebtsByUserIdAsync(userId, cancellationToken);

            foreach (var debt in unpaidDebts)
            {
                if (remainingAmount <= 0) break;

                decimal amountToPay = Math.Min(remainingAmount, debt.Remaining);

                debt.Remaining -= amountToPay;
                wallet.TotalDebt -= amountToPay;
                remainingAmount -= amountToPay;

                if (debt.Remaining == 0)
                {
                    debt.Status = "Paid";
                    debt.PaidAt = DateTimeOffset.UtcNow;
                }
                _walletDebtRepository.Update(debt);

                await _walletTransactionRepository.AddAsync(new WalletTransaction
                {
                    WalletId = wallet.Id,
                    Direction = "In",
                    Source = $"DebtRepayment_Order_{debt.OrderId}",
                    Amount = amountToPay,
                    BalanceAfter = wallet.Balance,
                    CreatedAt = DateTimeOffset.UtcNow
                }, cancellationToken);
            }

            if (remainingAmount > 0)
            {
                wallet.Balance += remainingAmount;
                await _walletTransactionRepository.AddAsync(new WalletTransaction
                {
                    WalletId = wallet.Id,
                    Direction = "In",
                    Source = source,
                    Amount = remainingAmount,
                    BalanceAfter = wallet.Balance,
                    CreatedAt = DateTimeOffset.UtcNow
                }, cancellationToken);
            }

            wallet.UpdatedAt = DateTimeOffset.UtcNow;
            _walletRepository.Update(wallet);
            // Không savechange(), service gọi nó (PaymentService) sẽ chịu trách nhiệm commit transaction.
        }
       
        public Task DebitAsync(long userId, decimal amount, string reason, long? orderId, CancellationToken cancellationToken)
        {
            // Logic trừ tiền 
            throw new NotImplementedException();
        }
    }
}
