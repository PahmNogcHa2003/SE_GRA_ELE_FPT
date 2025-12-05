using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Domain.Entities
{
    [Table("UserLifetimeStats")]
    [Index(nameof(UserId), IsUnique = true)]
    public class UserLifetimeStats : BaseEntity<long>
    {
        public long UserId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalDistanceKm { get; set; } = 0;

        public int TotalTrips { get; set; } = 0;

        public int TotalDurationMinutes { get; set; } = 0;

        [Column(TypeName = "decimal(18,4)")]
        public decimal TotalCo2SavedKg { get; set; } = 0;

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalCaloriesBurned { get; set; } = 0;

        [Precision(0)]
        public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;

        [ForeignKey(nameof(UserId))]
        public AspNetUser User { get; set; } = null!;
    }
}
