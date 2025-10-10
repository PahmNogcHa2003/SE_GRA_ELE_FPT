using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Domain.Entities;

[Table("KycDocument")]
[Microsoft.EntityFrameworkCore.Index(nameof(SubmissionId), Name = "IX_KycDocument_SubmissionId")]
public class KycDocument : BaseEntity<long>
{
    [Required]
    public long SubmissionId { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? DocType { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string? DocPath { get; set; }

    [Precision(0)]
    public DateTimeOffset UploadedAt { get; set; } = DateTimeOffset.UtcNow;

    // 🔗 Navigation property
    [ForeignKey(nameof(SubmissionId))]
    [InverseProperty(nameof(KycSubmission.KycDocuments))]
    public KycSubmission Submission { get; set; } = null!;
}
