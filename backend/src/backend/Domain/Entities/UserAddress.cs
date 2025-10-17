using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Domain.Entities;

[Table("UserAddress")]
[Index(nameof(UserId), Name = "IX_UserAddress_UserId")]
public partial class UserAddress
{
    [Key]
    public long AddressId { get; set; }

    [Required]
    public long UserId { get; set; }

    [StringLength(255)]
    public string? Line1 { get; set; }

    [StringLength(50)]
    public string? ProvinceCode { get; set; }

    [StringLength(50)]
    public string? DistrictCode { get; set; }

    [StringLength(50)]
    public string? WardCode { get; set; }

    [StringLength(100)]
    public string? WardName { get; set; }

    [StringLength(100)]
    public string? Country { get; set; }

    [Required]
    [Precision(0)]
    public DateTimeOffset CreatedAt { get; set; }

    [ForeignKey(nameof(UserId))]
    [InverseProperty(nameof(AspNetUser.UserAddresses))]
    public virtual AspNetUser User { get; set; } = null!;
}

