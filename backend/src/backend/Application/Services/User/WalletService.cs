using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Application.DTOs;
using Application.DTOs.Wallet;
using Application.Interfaces;
using Application.Interfaces.Base;
using Application.Interfaces.Staff.Repository;
using Application.Interfaces.User.Repository;
using Application.Interfaces.User.Service;
using Application.Services.Base;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Domain.Rules;

namespace Application.Services.User
{
    public class WalletService : GenericService<Wallet, WalletDTO, long>, IWalletService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWalletDebtRepository _walletDebtRepository;
        private readonly IWalletRepository _walletRepository;
        private readonly Application.Interfaces.User.Repository.IWalletTransactionRepository _walletTransactionRepository;

        public WalletService(
            IRepository<Wallet, long> repo,
            IMapper mapper,
            IUnitOfWork unitOfWork,
            IWalletDebtRepository walletDebtRepository,
            IWalletRepository walletRepository,
            Application.Interfaces.User.Repository.IWalletTransactionRepository walletTransactionRepository
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
                wallet = new Wallet 
                {
                    UserId = userId, 
                    Status = "Active", 
                    Balance = 0, 
                    TotalDebt = 0, 
                    PromoBalance = 0, 
                    UpdatedAt = DateTimeOffset.UtcNow 
                };
                await _walletRepository.AddAsync(wallet, cancellationToken);
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
                    debt.Status = WalletDebtStatus.Paid;
                    debt.PaidAt = DateTimeOffset.UtcNow;
                }
                _walletDebtRepository.Update(debt);

                lastTxn = new WalletTransaction
                {
                    WalletId = wallet.Id,
                    Direction = "In", 
                    Source = $"DebtRepayment_Order_{debt.OrderId}",
                    Amount = amountToPay,
                    BalanceAfter = wallet.Balance, 
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
                    Source = source, 
                    Amount = remainingAmount,
                    BalanceAfter = wallet.Balance,
                    CreatedAt = DateTimeOffset.UtcNow
                };
                await _walletTransactionRepository.AddAsync(lastTxn, cancellationToken);
            }

            // Cập nhật ví
            wallet.UpdatedAt = DateTimeOffset.UtcNow;
            _walletRepository.Update(wallet);

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
        public async Task<WalletTransaction> CreditPromoAsync(
            long userId,
            decimal amount,
            string source,
            CancellationToken ct)
        {
            if (amount <= 0)
                throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be greater than 0.");

            var now = DateTimeOffset.UtcNow;
            var wallet = await _walletRepository.GetByUserIdAsync(userId, ct);
            if (wallet == null)
            {
                wallet = new Wallet
                {
                    UserId = userId,
                    Balance = 0,
                    PromoBalance = 0,
                    TotalDebt = 0,
                    Status = "Active",
                    UpdatedAt = now
                };
                await _walletRepository.AddAsync(wallet, ct);
            }

            wallet.PromoBalance += amount;
            wallet.UpdatedAt = now;

            var txn = new WalletTransaction
            {
                WalletId = wallet.Id,
                Direction = "In",
                Source = source,                
                Amount = amount,
                BalanceAfter = wallet.Balance,
                PromoAfter = wallet.PromoBalance,
                CreatedAt = now
            };

            await _walletTransactionRepository.AddAsync(txn, ct);
            _walletRepository.Update(wallet);
            await _unitOfWork.SaveChangesAsync(ct);

            return txn;
        }


        public async Task<WalletTransaction> ConvertPromoToBalanceAsync(long userId, decimal amount, CancellationToken ct)
        {
            if (amount <= 0) throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be greater than 0.");
            var wallet = await _walletRepository.GetByUserIdAsync(userId, ct);

            if (wallet == null || wallet.Status != "Active")
                throw new InvalidOperationException("Wallet not found or not active.");

            if (wallet.PromoBalance < amount)
                throw new InvalidOperationException("Promo balance is not enough to convert.");

            wallet.PromoBalance -= amount;
            wallet.Balance += amount;
            wallet.UpdatedAt = DateTimeOffset.UtcNow;

            var txn = new WalletTransaction
            {
                WalletId = wallet.Id,
                Direction = "In",
                Source = "PromoConvert",
                Amount = amount,
                BalanceAfter = wallet.Balance,
                PromoAfter = wallet.PromoBalance,
                CreatedAt = DateTimeOffset.UtcNow
            };
            await _walletTransactionRepository.AddAsync(txn, ct);
            _walletRepository.Update(wallet);
            await _unitOfWork.SaveChangesAsync(ct);
            return txn;
        }
        public Task<WalletTransaction> DebitAsync(long userId, decimal amount, string reason, long? orderId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
