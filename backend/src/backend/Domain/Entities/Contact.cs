using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Domain.Entities;

[Table("Contact")]
[Microsoft.EntityFrameworkCore.Index(nameof(UserId), Name = "IX_Contact_UserId")]
public class Contact : BaseEntity<long>
{
    [Required]
    public long UserId { get; set; }

    [StringLength(500)]
    [Unicode(false)]
    public string? Message { get; set; }

    [Precision(0)]
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    // 🔗 Navigation property
    [ForeignKey(nameof(UserId))]
    [InverseProperty(nameof(AspNetUser.Contacts))]
    public AspNetUser User { get; set; } = null!;
}
