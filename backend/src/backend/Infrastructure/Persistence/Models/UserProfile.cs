using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Models;

[Table("UserProfile")]
public partial class UserProfile
{
    [Key]
    public long Id { get; set; }

    public long UserId { get; set; }

    [StringLength(150)]
    public string? FullName { get; set; }

    public DateOnly? Dob { get; set; }

    [StringLength(20)]
    public string? Gender { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("UserProfiles")]
    public virtual AspNetUser User { get; set; } = null!;
}
