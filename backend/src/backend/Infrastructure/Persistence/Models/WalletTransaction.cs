using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Models;

[Table("WalletTransaction")]
public partial class WalletTransaction
{
    [Key]
    public long Id { get; set; }

    public long WalletId { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal Amount { get; set; }

    [StringLength(50)]
    public string Type { get; set; } = null!;

    public DateTimeOffset CreatedAt { get; set; }

    [ForeignKey("WalletId")]
    [InverseProperty("WalletTransactions")]
    public virtual Wallet Wallet { get; set; } = null!;
}
