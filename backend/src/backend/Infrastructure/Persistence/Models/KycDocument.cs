using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Models;

[Table("KycDocument")]
public partial class KycDocument
{
    [Key]
    public long Id { get; set; }

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
