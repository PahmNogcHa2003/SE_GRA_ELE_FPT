using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Domain.Entities;

[Table("UserDevice")]
[Microsoft.EntityFrameworkCore.Index(nameof(UserId), Name = "IX_UserDevice_UserId")]
public partial class UserDevice : BaseEntity<long>
{
    public long UserId { get; set; }

    [StringLength(100)]
    public string? DeviceId { get; set; }

    [StringLength(50)]
    public string? DeviceType { get; set; }

    public DateTimeOffset? LastLogin { get; set; }

    [ForeignKey(nameof(UserId))]
    [InverseProperty(nameof(AspNetUser.UserDevices))]
    public virtual AspNetUser User { get; set; } = null!;
}
