using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Domain.Entities;

[Table("UserProfile")]
[Index("UserId", Name = "IX_UserProfile_UserId")]
public partial class UserProfile : BaseEntity<long>
{

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
