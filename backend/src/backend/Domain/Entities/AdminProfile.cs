using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Domain.Entities
{
    [Table("AdminProfile")]
    public class AdminProfile
    {
        [Key]
        public long UserId { get; set; }

        [StringLength(150)]
        public string? FullName { get; set; }

        [StringLength(100)]
        public string? Position { get; set; }

        [StringLength(250)]
        public string? AvatarUrl { get; set; }

        [Required]
        [Precision(0)]
        public DateTimeOffset CreatedAt { get; set; }

        [Required]
        [Precision(0)]
        public DateTimeOffset UpdatedAt { get; set; }

        [ForeignKey(nameof(UserId))]
        [InverseProperty("AdminProfile")]
        public virtual AspNetUser User { get; set; } = null!;
    }
}
