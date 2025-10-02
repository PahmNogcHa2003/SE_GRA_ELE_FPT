using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Domain.Entities;

[Table("KycSubmission")]
[Index("UserId", Name = "IX_KycSubmission_UserId")]
public partial class KycSubmission : BaseEntity<long>
{
    public long UserId { get; set; }

    [StringLength(50)]
    public string Status { get; set; } = null!;

    public DateTimeOffset SubmittedAt { get; set; }

    public DateTimeOffset? ReviewedAt { get; set; }

    [InverseProperty("Submission")]
    public virtual ICollection<KycDocument> KycDocuments { get; set; } = new List<KycDocument>();

    [InverseProperty("Submission")]
    public virtual ICollection<KycProfile> KycProfiles { get; set; } = new List<KycProfile>();

    [ForeignKey("UserId")]
    [InverseProperty("KycSubmissions")]
    public virtual AspNetUser User { get; set; } = null!;
}
