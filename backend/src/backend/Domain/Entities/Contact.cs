using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Domain.Entities;

[Table("Contact")]
public class Contact : BaseEntity<long>
{
    [StringLength(255)]
    public string? Email { get; set; }

    [StringLength(50)]
    public string? PhoneNumber { get; set; }

    [Required]
    [StringLength(150)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [StringLength(4000)]
    public string Content { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    [Unicode(false)]
    public string Status { get; set; } 
    public long? ReplyById { get; set; }

    [StringLength(4000)]
    public string? ReplyContent { get; set; }

    [Precision(0)]
    public DateTimeOffset? ReplyAt { get; set; }

    public bool IsReplySent { get; set; } = false;

    [Required]
    [Precision(0)]
    public DateTimeOffset CreatedAt { get; set; }

    [Precision(0)]
    public DateTimeOffset? ClosedAt { get; set; }

    [ForeignKey(nameof(ReplyById))]
    public AspNetUser? Reply { get; set; }
}
