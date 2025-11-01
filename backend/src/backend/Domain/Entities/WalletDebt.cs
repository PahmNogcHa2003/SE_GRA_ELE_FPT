using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Domain.Entities;

[Table("WalletDebt")]
[Index(nameof(UserId), nameof(OrderId), Name = "UQ_WalletDebt_User_Order", IsUnique = true)]
public partial class WalletDebt : BaseEntity<long>
{
    [Required]
    public long UserId { get; set; }

    public long? OrderId { get; set; }

    [Required]
    [Column(TypeName = "decimal(18, 2)")]
    public decimal Amount { get; set; }

    [Required]
    [Column(TypeName = "decimal(18, 2)")]
    public decimal Remaining { get; set; }

    [Required]
    [StringLength(20)]
    [Unicode(false)]
    public string Status { get; set; } = "Unpaid";

    [Required]
    [Precision(0)]
    public DateTimeOffset CreatedAt { get; set; }

    [Precision(0)]
    public DateTimeOffset? PaidAt { get; set; }

    [ForeignKey(nameof(UserId))]
    public virtual AspNetUser User { get; set; } = null!;

    [ForeignKey(nameof(OrderId))]
    public virtual Order? Order { get; set; }
}
