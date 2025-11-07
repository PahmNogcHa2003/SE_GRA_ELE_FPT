using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Transactions
{
    public class TransactionQueryParams
    {
        public long? UserId { get; set; }
        public string? TransactionType { get; set; }

        public string? OrderType { get; set; }   // Filter cho Order
        public string? Direction { get; set; }   // Filter cho WalletTransaction (IN, OUT)
        public string? Status { get; set; }      // Status của Order

        public DateTimeOffset? From { get; set; }
        public DateTimeOffset? To { get; set; }

        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
