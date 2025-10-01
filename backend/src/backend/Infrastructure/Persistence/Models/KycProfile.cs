using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Models;

[Table("KycProfile")]
public partial class KycProfile
{
    [Key]
    public long Id { get; set; }

    public long SubmissionId { get; set; }

    [StringLength(150)]
    public string? VerifiedName { get; set; }

    public DateOnly? VerifiedDob { get; set; }

    [StringLength(20)]
    public string? VerifiedGender { get; set; }

    public DateTimeOffset? VerifiedAt { get; set; }

    [ForeignKey("SubmissionId")]
    [InverseProperty("KycProfiles")]
    public virtual KycSubmission Submission { get; set; } = null!;
}
