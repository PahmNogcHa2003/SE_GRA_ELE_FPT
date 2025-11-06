using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Interfaces;
using Application.Interfaces.Base;
using Application.Interfaces.User.Repository;
using Application.Interfaces.User.Service;
using Application.Services.Base;
using AutoMapper;
using Domain.Entities;

namespace Application.Services.User
{
    public class WalletService : GenericService<Wallet, WalletDTO, long>, IWalletService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWalletDebtRepository _walletDebtRepository;
        private readonly IWalletRepository _walletRepository;
        private readonly IWalletTransactionRepository _walletTransactionRepository;

        public WalletService(
            IRepository<Wallet, long> repo,
            IMapper mapper,
            IUnitOfWork unitOfWork,
            IWalletDebtRepository walletDebtRepository,
            IWalletRepository walletRepository,
            IWalletTransactionRepository walletTransactionRepository
        ) : base(repo, mapper, unitOfWork)    
        {
            _unitOfWork = unitOfWork;
            _walletDebtRepository = walletDebtRepository;
            _walletRepository = walletRepository;
            _walletTransactionRepository = walletTransactionRepository;
        }
        public async Task<WalletTransaction> CreditAsync(long userId, decimal amount, string source, long? orderId, CancellationToken cancellationToken)
        {
            if (amount <= 0) throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be greater than 0.");

            var wallet = await _walletRepository.GetByUserIdAsync(userId, cancellationToken);
            if (wallet == null)
            {
                wallet = new Wallet { UserId = userId, Status = "Active", Balance = 0, TotalDebt = 0, UpdatedAt = DateTimeOffset.UtcNow };
                await _walletRepository.AddAsync(wallet, cancellationToken);
                // KHÔNG SaveChanges ở đây – để caller gói transaction & commit
            }

            decimal remainingAmount = amount;
            WalletTransaction? lastTxn = null;

            // 1) Trả nợ trước nếu có
            var unpaidDebts = await _walletDebtRepository.GetUnpaidDebtsByUserIdAsync(userId, cancellationToken);
            foreach (var debt in unpaidDebts)
            {
                if (remainingAmount <= 0) break;

                var amountToPay = Math.Min(remainingAmount, debt.Remaining);
                debt.Remaining -= amountToPay;
                wallet.TotalDebt -= amountToPay;
                remainingAmount -= amountToPay;

                if (debt.Remaining == 0)
                {
                    debt.Status = "Paid";
                    debt.PaidAt = DateTimeOffset.UtcNow;
                }
                _walletDebtRepository.Update(debt);

                // Ghi nhận transaction cho phần trả nợ (không làm thay đổi Balance)
                lastTxn = new WalletTransaction
                {
                    WalletId = wallet.Id,
                    Direction = "In", // chuẩn hoá "IN"/"OUT"
                    Source = $"DebtRepayment_Order_{debt.OrderId}",
                    Amount = amountToPay,
                    BalanceAfter = wallet.Balance, // không thay đổi số dư ví
                    CreatedAt = DateTimeOffset.UtcNow
                };
                await _walletTransactionRepository.AddAsync(lastTxn, cancellationToken);
            }

            // 2) Phần còn lại +Balance
            if (remainingAmount > 0)
            {
                wallet.Balance += remainingAmount;
                lastTxn = new WalletTransaction
                {
                    WalletId = wallet.Id,
                    Direction = "In",
                    Source = source, // "VNPay (Order 123)" nếu có order
                    Amount = remainingAmount,
                    BalanceAfter = wallet.Balance,
                    CreatedAt = DateTimeOffset.UtcNow
                };
                await _walletTransactionRepository.AddAsync(lastTxn, cancellationToken);
            }

            // Cập nhật ví
            wallet.UpdatedAt = DateTimeOffset.UtcNow;
            _walletRepository.Update(wallet);

            // KHÔNG SaveChanges ở đây – caller (PaymentService) sẽ Save & Commit
            // Đảm bảo luôn trả về 1 transaction có ý nghĩa để FE hiện hóa đơn
            // Nếu toàn bộ tiền dùng để trả nợ, lastTxn là repayment cuối cùng (BalanceAfter = Balance không đổi)
            return lastTxn ?? new WalletTransaction
            {
                WalletId = wallet.Id,
                Direction = "In",
                Source = source,
                Amount = 0,
                BalanceAfter = wallet.Balance,
                CreatedAt = DateTimeOffset.UtcNow
            };
        }

        public Task<WalletTransaction> DebitAsync(long userId, decimal amount, string reason, long? orderId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
