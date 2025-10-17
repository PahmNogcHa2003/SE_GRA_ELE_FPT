using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Domain.Entities;

[Table("Contact")]
public class Contact : BaseEntity<long>
{
    public long? UserId { get; set; }

    [StringLength(255)]
    public string? Email { get; set; }

    [StringLength(50)]
    public string? PhoneNumber { get; set; }

    [Required]
    [StringLength(150)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [StringLength(1000)]
    public string Content { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    [Unicode(false)] 
    public string Status { get; set; } = "Open"; 

    public long? AssignedTo { get; set; }

    [Required]
    [Precision(0)]
    public DateTimeOffset CreatedAt { get; set; } 

    [Precision(0)]
    public DateTimeOffset? ClosedAt { get; set; }

    [ForeignKey(nameof(UserId))]
    [InverseProperty(nameof(AspNetUser.Contacts))]
    public AspNetUser? User { get; set; } 
   
    [ForeignKey(nameof(AssignedTo))]
    public AspNetUser? Assignee { get; set; }
}