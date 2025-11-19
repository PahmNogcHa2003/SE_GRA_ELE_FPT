using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Domain.Entities;

[Table("UserProfile")]
public partial class UserProfile : BaseEntity<long>
{

    [ForeignKey(nameof(User))]
    public long UserId { get; set; }

    [StringLength(150)]
    public string? FullName { get; set; }

    [Column(TypeName = "date")]
    public DateTime? Dob { get; set; }

    [StringLength(10)]
    [Unicode(false)]
    public string? Gender { get; set; }

    [StringLength(255)]
    public string? AvatarUrl { get; set; }

    [StringLength(200)]
    public string? AvatarPublicId { get; set; }

    public int? ProvinceCode { get; set; }
    [StringLength(50)]
    public string? ProvinceName { get; set; }
    public int? WardCode { get; set; }

    [StringLength(100)]
    public string? WardName { get; set; }

    public string? AddressDetail { get; set; }

    [StringLength(150)]
    public string? EmergencyName { get; set; }

    [Required]
    [StringLength(15)]
    public string EmergencyPhone { get; set; } // ĐÃ SỬA: string

    [Required]
    [StringLength(15)]
    public string NumberCard { get; set; } // ĐÃ SỬA: string

    [StringLength(150)]
    public string? PlaceOfOrigin { get; set; }

    [StringLength(150)]
    public string? PlaceOfResidence { get; set; }

    [Precision(7)]
    public DateTimeOffset? IssuedDate { get; set; }

    [Precision(7)]
    public DateTimeOffset? ExpiryDate { get; set; }

    [StringLength(100)]
    public string? IssuedBy { get; set; }

    [Required]
    [Precision(0)]
    public DateTimeOffset CreatedAt { get; set; }

    [Required]
    [Precision(0)]
    public DateTimeOffset UpdatedAt { get; set; }

    public virtual AspNetUser User { get; set; } = null!;
}