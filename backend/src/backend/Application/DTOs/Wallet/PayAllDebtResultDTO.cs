using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Wallet
{
    public class PayDebtResultDTO
    {
        public decimal PaidAmount { get; set; }
        public decimal RemainingDebt { get; set; }
        public decimal WalletBalanceAfter { get; set; }
    }
}
