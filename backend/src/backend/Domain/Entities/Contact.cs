using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Domain.Entities;

[Table("Contact")]
[Index("UserId", Name = "IX_Contact_UserId")]
public partial class Contact : BaseEntity<long>
{

    public long UserId { get; set; }

    [StringLength(500)]
    public string? Message { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("Contacts")]
    public virtual AspNetUser User { get; set; } = null!;
}
