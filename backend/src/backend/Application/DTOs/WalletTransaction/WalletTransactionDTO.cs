using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.WalletTransaction
{
    public class WalletTransactionDTO
    {
        public long Id { get; set; }
        public long WalletId { get; set; } 
        public string Direction { get; set; } = string.Empty; 
        public decimal Amount { get; set; }
        public string Source { get; set; } = string.Empty; 
        public decimal BalanceAfter { get; set; } 
        public DateTimeOffset CreatedAt { get; set; }
    }
}
