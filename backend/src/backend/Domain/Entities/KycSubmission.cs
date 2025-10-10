using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Domain.Entities;

[Table("KycSubmission")]
[Microsoft.EntityFrameworkCore.Index(nameof(UserId), Name = "IX_KycSubmission_UserId")]
public class KycSubmission : BaseEntity<long>
{
    [Required]
    public long UserId { get; set; }

    [Required]
    [StringLength(50)]
    [Unicode(false)] // ⚙️ nếu không cần lưu tiếng Việt
    public string Status { get; set; } = null!;

    [Precision(0)]
    public DateTimeOffset SubmittedAt { get; set; }

    [Precision(0)]
    public DateTimeOffset? ReviewedAt { get; set; }

    // 🔗 Relationships
    [InverseProperty(nameof(KycDocument.Submission))]
    public ICollection<KycDocument> KycDocuments { get; set; } = new List<KycDocument>();

    [InverseProperty(nameof(KycProfile.Submission))]
    public ICollection<KycProfile> KycProfiles { get; set; } = new List<KycProfile>();

    [ForeignKey(nameof(UserId))]
    [InverseProperty(nameof(AspNetUser.KycSubmissions))]
    public AspNetUser User { get; set; } = null!;
}
