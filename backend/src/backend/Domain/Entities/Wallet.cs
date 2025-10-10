using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Domain.Entities;

[Table("Wallet")]
[Microsoft.EntityFrameworkCore.Index(nameof(UserId), Name = "IX_Wallet_UserId")]
public partial class Wallet : BaseEntity<long>
{
    public long UserId { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal Balance { get; set; }

    public DateTimeOffset UpdatedAt { get; set; }

    [ForeignKey(nameof(UserId))]
    [InverseProperty(nameof(AspNetUser.Wallets))]
    public virtual AspNetUser User { get; set; } = null!;

    [InverseProperty(nameof(WalletTransaction.Wallet))]
    public virtual ICollection<WalletTransaction> WalletTransactions { get; set; } = new List<WalletTransaction>();
}
