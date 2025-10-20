using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces;
using Application.Interfaces.User.Service;
using Domain.Entities;

namespace Application.Services.User
{
    public class WalletService : IWalletService
    {
        private readonly IUnitOfWork _unitOfWork;
        public WalletService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task CreditAsync(long userId, decimal amount, string source, long? orderId, CancellationToken cancellationToken)
        {
            var wallet = await _unitOfWork.Wallets.GetByUserIdAsync(userId, cancellationToken);

            if (wallet == null)
            {
                wallet = new Wallet { UserId = userId, Status = "Active" };
                await _unitOfWork.Wallets.AddAsync(wallet, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }

            decimal remainingAmount = amount;

            var unpaidDebts = await _unitOfWork.WalletDebts.GetUnpaidDebtsByUserIdAsync(userId, cancellationToken);

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
                _unitOfWork.WalletDebts.Update(debt);

                await _unitOfWork.WalletTransactions.AddAsync(new WalletTransaction
                {
                    WalletId = wallet.UserId,
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
                await _unitOfWork.WalletTransactions.AddAsync(new WalletTransaction
                {
                    WalletId = wallet.UserId,
                    Direction = "In",
                    Source = source,
                    Amount = remainingAmount,
                    BalanceAfter = wallet.Balance,
                    CreatedAt = DateTimeOffset.UtcNow
                }, cancellationToken);
            }

            wallet.UpdatedAt = DateTimeOffset.UtcNow;
            _unitOfWork.Wallets.Update(wallet);
            // Không savechange(), service gọi nó (PaymentService) sẽ chịu trách nhiệm commit transaction.
        }
       
        public Task DebitAsync(long userId, decimal amount, string reason, long? orderId, CancellationToken cancellationToken)
        {
            // Logic trừ tiền 
            throw new NotImplementedException();
        }
    }
}
