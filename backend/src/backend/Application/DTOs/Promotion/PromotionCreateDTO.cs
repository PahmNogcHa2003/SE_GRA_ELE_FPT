using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Promotion
{
    public class PromotionCreateDTO
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal MinAmount { get; set; }
        public decimal BonusPercent { get; set; }
        public DateTimeOffset StartAt { get; set; }
        public DateTimeOffset EndAt { get; set; }
    }
}
