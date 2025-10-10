using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Domain.Entities;

[Table("WalletTransaction")]
[Microsoft.EntityFrameworkCore.Index(nameof(WalletId), Name = "IX_WalletTransaction_WalletId")]
public partial class WalletTransaction : BaseEntity<long>
{
    public long WalletId { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal Amount { get; set; }

    [StringLength(50)]
    public string Type { get; set; } = null!;

    public DateTimeOffset CreatedAt { get; set; }

    [ForeignKey(nameof(WalletId))]
    [InverseProperty(nameof(Wallet.WalletTransactions))]
    public virtual Wallet Wallet { get; set; } = null!;
}
