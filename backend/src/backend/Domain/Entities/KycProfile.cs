using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Domain.Entities;

[Table("KycProfile")]
[Microsoft.EntityFrameworkCore.Index(nameof(SubmissionId), Name = "IX_KycProfile_SubmissionId")]
public class KycProfile : BaseEntity<long>
{
    [Required]
    public long SubmissionId { get; set; }

    [StringLength(150)]
    [Unicode(false)]
    public string? VerifiedName { get; set; }

    public DateOnly? VerifiedDob { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string? VerifiedGender { get; set; }

    [Precision(0)]
    public DateTimeOffset? VerifiedAt { get; set; }

    // 🔗 Navigation
    [ForeignKey(nameof(SubmissionId))]
    [InverseProperty(nameof(KycSubmission.KycProfiles))]
    public KycSubmission Submission { get; set; } = null!;
}
