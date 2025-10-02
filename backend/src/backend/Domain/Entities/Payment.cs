using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Domain.Entities;

[Table("Payment")]
[Index("OrderId", Name = "IX_Payment_OrderId")]
public partial class Payment : BaseEntity<long>
{
    public long OrderId { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal Amount { get; set; }

    [StringLength(50)]
    public string Method { get; set; } = null!;

    [StringLength(20)]
    public string Status { get; set; } = null!;

    public DateTimeOffset CreatedAt { get; set; }

    [ForeignKey("OrderId")]
    [InverseProperty("Payments")]
    public virtual Order Order { get; set; } = null!;
}
