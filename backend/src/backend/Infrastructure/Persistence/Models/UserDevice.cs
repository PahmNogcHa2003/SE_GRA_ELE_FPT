using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Models;

[Table("UserDevice")]
public partial class UserDevice
{
    [Key]
    public long Id { get; set; }

    public long UserId { get; set; }

    [StringLength(100)]
    public string? DeviceId { get; set; }

    [StringLength(50)]
    public string? DeviceType { get; set; }

    public DateTimeOffset? LastLogin { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("UserDevices")]
    public virtual AspNetUser User { get; set; } = null!;
}
