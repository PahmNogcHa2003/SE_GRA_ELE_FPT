using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Models;

[Table("UserSession")]
public partial class UserSession
{
    [Key]
    public long Id { get; set; }

    public long UserId { get; set; }

    [StringLength(200)]
    public string? SessionToken { get; set; }

    public DateTimeOffset? Expiry { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("UserSessions")]
    public virtual AspNetUser User { get; set; } = null!;
}
