using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Wallet
{
    public class WalletDTO
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public decimal Balance { get; set; }
        public decimal TotalDebt { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
    }
}
