using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Rules
{
    public static class PromoRules
    {
        public static decimal CalculateBonus(decimal topupAmount)
        {
            if (topupAmount >= 500000) return topupAmount * 0.20m; // 20%
            if (topupAmount >= 200000) return topupAmount * 0.15m; // 15%
            if (topupAmount >= 100000) return topupAmount * 0.10m; // 10%
            return 0;
        }
    }
}
