using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Domain.Entities;

[Table("KycForm")]
[Index(nameof(UserId), IsUnique = true, Name = "UQ_KycForm_User")]
public class KycForm : BaseEntity<long>
{
    [Required]
    public long UserId { get; set; }

    [StringLength(100)]
    public string? NumberCard { get; set; }

    [StringLength(255)]
    public string? IdFrontUrl { get; set; }

    [StringLength(255)]
    public string? IdBackUrl { get; set; }

    [Required]
    [StringLength(20)]
    public string Status { get; set; }

    [Precision(0)]
    public DateTimeOffset? SubmittedAt { get; set; }

    // 🔗 Navigation property
    [ForeignKey(nameof(UserId))]
    [InverseProperty(nameof(AspNetUser.KycForms))]
    public AspNetUser User { get; set; } = null!;
}