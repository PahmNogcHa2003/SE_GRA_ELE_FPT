using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Domain.Entities;

[Table("KycDocument")]
[Index("SubmissionId", Name = "IX_KycDocument_SubmissionId")]
public partial class KycDocument : BaseEntity<long>
{
    public long SubmissionId { get; set; }

    [StringLength(50)]
    public string? DocType { get; set; }

    [StringLength(255)]
    public string? DocPath { get; set; }

    public DateTimeOffset UploadedAt { get; set; }

    [ForeignKey("SubmissionId")]
    [InverseProperty("KycDocuments")]
    public virtual KycSubmission Submission { get; set; } = null!;
}
