using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Interfaces.User.Service;
using Application.Interfaces;
using Application.Services.Base;
using AutoMapper;
using Domain.Entities;
using Application.Interfaces.Base;
using Microsoft.EntityFrameworkCore;
using Application.Common;
using AutoMapper.QueryableExtensions;

namespace Application.Services.User
{
    public class WalletTransactionService : GenericService<WalletTransaction, WalletTransactionDTO, long>, IWalletTransactionService
    {
        public WalletTransactionService(IRepository<WalletTransaction,long> repo,IUnitOfWork uow, IMapper mapper)
            : base(repo, mapper, uow)
        {
        }

        public async Task<PagedResult<WalletTransactionDTO>> GetTransactionsByUserIdAsync(
            long userId,
            int page,
            int pageSize,
            string? sortOrder,
            CancellationToken ct = default)
        {
            var query = _repo.Query()
                             .AsNoTracking()
                             .Include(t => t.Wallet) 
                             .Where(t => t.Wallet.UserId == userId);

            if (string.IsNullOrWhiteSpace(sortOrder))
            {
                sortOrder = "createdAt_desc";
            }
            query = ApplySort(query, sortOrder);

            var projectedQuery = query.ProjectTo<WalletTransactionDTO>(_mapper.ConfigurationProvider);

            return await PagedResult<WalletTransactionDTO>.FromQueryableAsync(projectedQuery, page, pageSize, ct);
        }
    }
}
