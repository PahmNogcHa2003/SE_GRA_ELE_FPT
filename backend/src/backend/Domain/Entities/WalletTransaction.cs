using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Domain.Entities;

[Table("WalletTransaction")]
[Index(nameof(WalletId), Name = "IX_WalletTransaction_WalletId")]
public partial class WalletTransaction : BaseEntity<long>
{
    [Required]
    public long WalletId { get; set; }

    [Required]
    [StringLength(6)]
    [Unicode(false)]
    public string Direction { get; set; } = null!;

    [Required]
    [StringLength(30)]
    [Unicode(false)]
    public string Source { get; set; } = null!;

    [Required]
    [Column(TypeName = "decimal(18, 2)")]
    public decimal Amount { get; set; }

    [Required]
    [Column(TypeName = "decimal(18, 2)")]
    public decimal BalanceAfter { get; set; }

    [Required]
    [Precision(0)]
    public DateTimeOffset CreatedAt { get; set; }

    [ForeignKey(nameof(WalletId))]
    [InverseProperty(nameof(Wallet.WalletTransactions))]
    public virtual Wallet Wallet { get; set; } = null!;
}
