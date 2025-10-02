using System;
using System.Collections.Generic;

namespace Domain.Entities.Models
{
    public class Wallet : BaseEntity<long>  
    {
        public long UserId { get; set; }
        public decimal Balance { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }

        // Navigation properties
        public virtual AspNetUser User { get; set; } = null!;
        public virtual ICollection<WalletTransaction> WalletTransactions { get; set; } = new List<WalletTransaction>();

        private Wallet() { }
    }
}
