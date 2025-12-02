using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Domain.Entities
{
    [Table("RentalHistory")]
    public class RentalHistory : BaseEntity<long>
    {
        [Required]
        public long RentalId { get; set; }

        [ForeignKey(nameof(RentalId))]
        public Rental Rental { get; set; } = null!;

        [Required]
        [Precision(0)]
        public DateTimeOffset Timestamp { get; set; }

        [Required]
        [StringLength(50)]
        [Unicode(false)]
        public string ActionType { get; set; } = string.Empty;

        public int? DurationMinutes { get; set; }
        public double? DistanceMeters { get; set; }
        public double? Co2SavedKg { get; set; }
        public double? CaloriesBurned { get; set; }

        [Unicode(true)]
        [StringLength(255)]
        public string? Description { get; set; }
    }
}
