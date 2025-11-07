using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.Transactions;
using Application.Interfaces.Staff.Repository;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Application.Common;
using Application.Interfaces.Staff.Service;

namespace Application.Services.Staff
{
    public class TransactionService : ITransactionService
    {
        private readonly IOrderRepository _orderRepo;
        private readonly IWalletTransactionRepository _walletTxRepo;
        public TransactionService(IOrderRepository orderRepo, IWalletTransactionRepository walletTxRepo)
        {
            _orderRepo = orderRepo;
            _walletTxRepo = walletTxRepo;
        }
        public async Task<Common.PagedResult<TransactionsDTO>> GetTransactionAsync(TransactionQueryParams query, CancellationToken ct = default)
        {
            var ordersQuery = _orderRepo.Query();
            if (query.UserId.HasValue)
            {
                ordersQuery = ordersQuery.Where(o => o.UserId == query.UserId.Value);
            }

            if (!string.IsNullOrWhiteSpace(query.OrderType))
            {
                ordersQuery = ordersQuery.Where(o => o.OrderType == query.OrderType);
            }

            if (!string.IsNullOrWhiteSpace(query.Status))
            {
                ordersQuery = ordersQuery.Where(o => o.Status == query.Status);
            }

            if (query.From.HasValue)
            {
                ordersQuery = ordersQuery.Where(o => o.CreatedAt >= query.From.Value);
            }

            if (query.To.HasValue)
            {
                ordersQuery = ordersQuery.Where(o => o.CreatedAt <= query.To.Value);
            }

            var orderItemsQuery = ordersQuery.Select(o => new TransactionsDTO
            {
                Id = o.Id,
                UserId = o.UserId,
                TransactionType = "ORDER",
                OrderNo = o.OrderNo,
                OrderType = o.OrderType,
                Direction = null,
                Source = "Order",
                Amount = o.Total,
                BalanceAfter = null,
                Currency = o.Currency,
                Status = o.Status,
                CreatedAt = o.CreatedAt,
                PaidAt = o.PaidAt
            });

            // WalletTransactions
            IQueryable<WalletTransaction> walletQuery = _walletTxRepo
                .Query()
                .Include(wt => wt.Wallet);

            if (query.UserId.HasValue)
            {
                walletQuery = walletQuery.Where(wt => wt.Wallet.UserId == query.UserId.Value);
            }
            if (query.From.HasValue)
            {
                walletQuery = walletQuery.Where(wt => wt.CreatedAt >= query.From.Value);
            }
            if (query.To.HasValue)
            {
                walletQuery = walletQuery.Where(wt => wt.CreatedAt <= query.To.Value);
            }

            var walletItemsQuery = walletQuery.Select(wt => new TransactionsDTO
            {
                Id = wt.Id,
                UserId = wt.Wallet.UserId,
                TransactionType = "WALLET",
                OrderNo = null,
                OrderType = null,
                Direction = wt.Direction,
                Source = wt.Source,
                Amount = wt.Amount,
                BalanceAfter = wt.BalanceAfter,
                Currency = "VND",
                Status = null,
                CreatedAt = wt.CreatedAt,
                PaidAt = null
            });

            // KẾT HỢP
            IQueryable<TransactionsDTO> combinedQuery;

            var type = query.TransactionType?.Trim().ToUpperInvariant();

            if (type == "ORDER")
            {
                combinedQuery = orderItemsQuery;
            }
            else if (type == "WALLET")
            {
                combinedQuery = walletItemsQuery;
            }
            else
            {
                combinedQuery = orderItemsQuery.Concat(walletItemsQuery);
            }

            combinedQuery = combinedQuery.OrderByDescending(t => t.CreatedAt);

            // PHÂN TRANG VÀ TRẢ VỀ
            return await Common.PagedResult<TransactionsDTO>.FromQueryableAsync(
                combinedQuery,
                query.Page,
                query.PageSize,
                ct
            );
        }
    }
}
