using System;

namespace Domain.Entities.Models
{
    public class WalletTransaction : BaseEntity<long>
    {
        public long WalletId { get; set; }
        public decimal Amount { get; set; }
        public string Type { get; set; } = null!;
        public DateTimeOffset CreatedAt { get; set; }

        // Navigation property
        public virtual Wallet Wallet { get; set; } = null!;

        private WalletTransaction() { }
    }
}
