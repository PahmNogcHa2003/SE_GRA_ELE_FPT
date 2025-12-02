using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    [Table("PromotionCampaign")]
    public class PromotionCampaign : BaseEntity<long>
    {
        [Required, StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(255)]
        public string? Description { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal MinAmount { get; set; } // Số tiền tối thiểu để áp dụng

        [Column(TypeName = "decimal(5,2)")]
        public decimal BonusPercent { get; set; } // % thưởng

        public DateTimeOffset StartAt { get; set; }
        public DateTimeOffset EndAt { get; set; }

        [StringLength(20)]
        public string Status { get; set; } = "Active"; // Active / Inactive
    }
}
