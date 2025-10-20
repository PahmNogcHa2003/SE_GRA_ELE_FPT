using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Domain.Entities;

[Table("Wallet")]
public partial class Wallet : BaseEntity<long>
{
    [Key]
    public long UserId { get; set; }

    [Required]
    [Column(TypeName = "decimal(18, 2)")]
    public decimal Balance { get; set; } = 0;

    [Required]
    [StringLength(3)]
    [Column(TypeName = "char(3)")]
    public string Currency { get; set; } = "VND";

    [Required]
    [StringLength(20)]
    [Unicode(false)]
    public string Status { get; set; } = "Active";

    [Required]
    [Column(TypeName = "decimal(18, 2)")]
    public decimal TotalDebt { get; set; } = 0;

    [Timestamp]
    public byte[]? RowVer { get; set; }

    [Precision(0)]
    public DateTimeOffset? UpdatedAt { get; set; }

    [ForeignKey(nameof(UserId))]
    [InverseProperty(nameof(AspNetUser.Wallet))]
    public virtual AspNetUser User { get; set; } = null!;

    [InverseProperty(nameof(WalletTransaction.Wallet))]
    public virtual ICollection<WalletTransaction> WalletTransactions { get; set; } = new List<WalletTransaction>();
}

