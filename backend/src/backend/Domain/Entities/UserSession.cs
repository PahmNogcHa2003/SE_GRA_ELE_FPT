using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Domain.Entities;

[Table("UserSession")]
[Microsoft.EntityFrameworkCore.Index(nameof(UserId), Name = "IX_UserSession_UserId")]
public partial class UserSession : BaseEntity<long>
{
    public long UserId { get; set; }

    [StringLength(200)]
    public string? SessionToken { get; set; }

    public DateTimeOffset? Expiry { get; set; }

    [ForeignKey(nameof(UserId))]
    [InverseProperty(nameof(AspNetUser.UserSessions))]
    public virtual AspNetUser User { get; set; } = null!;
}
