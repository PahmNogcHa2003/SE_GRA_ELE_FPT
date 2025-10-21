using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Domain.Entities;

[Table("UserDevice")]
[Index(nameof(UserId), nameof(DeviceId), IsUnique = true, Name = "UQ_UserDevice_User_Device")]
public partial class UserDevice : BaseEntity<long>
{
    [Required]
    public long UserId { get; set; }

    [Required]
    public Guid DeviceId { get; set; }

    [StringLength(50)]
    public string? Platform { get; set; } = "ANDROID";

    [StringLength(1024)]
    public string? PushToken { get; set; }

    [Precision(0)]
    public DateTimeOffset? LastLoginAt { get; set; }

    [Required]
    [Precision(0)]
    public DateTimeOffset CreatedAt { get; set; }

    [Required]
    [Precision(0)]
    public DateTimeOffset UpdatedAt { get; set; }

    [ForeignKey(nameof(UserId))]
    [InverseProperty(nameof(AspNetUser.UserDevices))]
    public virtual AspNetUser User { get; set; } = null!;
}
