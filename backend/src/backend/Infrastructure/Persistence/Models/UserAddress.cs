using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Models;

[Table("UserAddress")]
public partial class UserAddress
{
    [Key]
    public long Id { get; set; }

    public long UserId { get; set; }

    [StringLength(255)]
    public string? Address { get; set; }

    [StringLength(100)]
    public string? City { get; set; }

    [StringLength(100)]
    public string? Country { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("UserAddresses")]
    public virtual AspNetUser User { get; set; } = null!;
}
