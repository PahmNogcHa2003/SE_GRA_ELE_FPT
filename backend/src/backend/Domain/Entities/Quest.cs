using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Domain.Entities
{
    [Table("Quest")]
    public class Quest : BaseEntity<long>
    {
        [Required, StringLength(50)]
        [Unicode(false)]
        public string Code { get; set; } = string.Empty;   // VD: "WEEKLY_10KM"

        [Required, StringLength(150)]
        public string Title { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        // "Distance" / "Trips"
        [Required, StringLength(20)]
        [Unicode(false)]
        public string QuestType { get; set; } = "Distance";

        // "Daily" / "Weekly" / "Monthly" / "OneTime"
        [Required, StringLength(20)]
        [Unicode(false)]
        public string Scope { get; set; } = "Weekly";

        // Mục tiêu
        [Column(TypeName = "decimal(18,2)")]
        public decimal? TargetDistanceKm { get; set; }  // cho Distance

        public int? TargetTrips { get; set; }           // cho Trips

        public int? TargetDurationMinutes { get; set; }

        // Phần thưởng promo (điểm)
        [Column(TypeName = "decimal(18,2)")]
        public decimal PromoReward { get; set; }

        public DateTimeOffset StartAt { get; set; }
        public DateTimeOffset EndAt { get; set; }

        [Required, StringLength(20)]
        [Unicode(false)]
        public string Status { get; set; } = "Active"; // Active / Inactive

        [Precision(0)]
        public DateTimeOffset? UpdatedAt { get; set; }

    }
}
