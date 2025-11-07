using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Transactions
{
    public class TransactionsDTO
    {
        public long Id { get; set; }              // Id của Order hoặc WalletTransaction
        public long UserId { get; set; }          // Chủ sở hữu

        public string TransactionType { get; set; } = null!;

        public string? OrderNo { get; set; }      // Chỉ có với Order
        public string? OrderType { get; set; }    // VD: "TicketPurchase", "Topup", ...

        public string? Direction { get; set; }    // IN/OUT (WalletTransaction)
        public string? Source { get; set; }       // VD: "Topup", "RideFee", "Penalty"...

        public decimal Amount { get; set; }       // Số tiền giao dịch
        public decimal? BalanceAfter { get; set; } // Số dư sau giao dịch (WalletTransaction)

        public string? Currency { get; set; }     // "VND"... (Order)
        public string? Status { get; set; }       // Status của Order (Pending, Paid, Cancelled...)

        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? PaidAt { get; set; }  // Order mới có
    }
}
