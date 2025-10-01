using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Models;

[Table("Wallet")]
public partial class Wallet
{
    [Key]
    public long Id { get; set; }

    public long UserId { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal Balance { get; set; }

    public DateTimeOffset UpdatedAt { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("Wallets")]
    public virtual AspNetUser User { get; set; } = null!;

    [InverseProperty("Wallet")]
    public virtual ICollection<WalletTransaction> WalletTransactions { get; set; } = new List<WalletTransaction>();
}
