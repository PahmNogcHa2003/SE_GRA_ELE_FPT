using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Domain.Entities;

[Table("KycForm")]
// Thêm Index Unique để khớp với "UQ_KycForm_User"
[Index(nameof(UserId), IsUnique = true, Name = "UQ_KycForm_User")]
public class KycForm : BaseEntity<long>
{
    [Required]
    public long UserId { get; set; }

    [StringLength(150)]
    public string? FullName { get; set; }

    // Dùng TypeName = "date" để khớp với kiểu DATE trong SQL
    [Column(TypeName = "date")]
    public DateTime? Dob { get; set; }

    [StringLength(10)]
    public string? Gender { get; set; }

    [StringLength(100)]
    public string? NumberCard { get; set; }

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

    [StringLength(255)]
    public string? IdFrontUrl { get; set; }

    [StringLength(255)]
    public string? IdBackUrl { get; set; }

    [StringLength(255)]
    public string? SelfieUrl { get; set; }

    [Required]
    [StringLength(20)]
    public string Status { get; set; } = "Pending"; 

    [StringLength(255)]
    public string? RejectReason { get; set; }

    [Precision(0)]
    public DateTimeOffset? SubmittedAt { get; set; }

    [Precision(0)]
    public DateTimeOffset? ReviewedAt { get; set; }

    [Required]
    [Precision(0)]
    public DateTimeOffset CreatedAt { get; set; }

    [Required]
    [Precision(0)]
    public DateTimeOffset UpdatedAt { get; set; } 

    // 🔗 Navigation property
    [ForeignKey(nameof(UserId))]
    [InverseProperty(nameof(AspNetUser.KycForms))] // Giả sử trong AspNetUser có ICollection<KycForm> KycForms
    public AspNetUser User { get; set; } = null!;
}