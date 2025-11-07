using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common;
using Application.DTOs;
using Application.DTOs.WalletTransaction;
using Application.Interfaces.Base;
using Domain.Entities;

namespace Application.Interfaces.User.Service
{
    public interface IWalletTransactionService : IService<WalletTransaction, WalletTransactionDTO, long>
    {
        Task<PagedResult<WalletTransactionDTO?>> GetTransactionsByUserIdAsync(
            long? userId,
            int page,
            int pageSize,
            string? sortOrder,
            CancellationToken ct = default);
    }
}
