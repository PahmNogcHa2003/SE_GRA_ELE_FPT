using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.Transactions;
using Application.Common;

namespace Application.Interfaces.Staff.Service
{
    public interface ITransactionService
    {
        Task<PagedResult<TransactionsDTO>> GetTransactionAsync (TransactionQueryParams query, CancellationToken ct = default);
    }
}
