using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Models;

[Table("Contact")]
public partial class Contact
{
    [Key]
    public long Id { get; set; }

    public long UserId { get; set; }

    [StringLength(500)]
    public string? Message { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("Contacts")]
    public virtual AspNetUser User { get; set; } = null!;
}
