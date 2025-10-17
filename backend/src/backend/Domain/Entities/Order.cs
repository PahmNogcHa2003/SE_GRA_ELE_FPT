using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Domain.Entities;

[Table("Order")]
[Index(nameof(UserId), Name = "IX_Order_UserId")]
public partial class Order : BaseEntity<long>
{
    [Required]
    public long UserId { get; set; }

    [StringLength(30)]
    public string? OrderNo { get; set; }

    [Required]
    [StringLength(30)]
    [Unicode(false)] 
    public string OrderType { get; set; } = null!;

    [Required]
    [StringLength(30)]
    [Unicode(false)] 
    public string Status { get; set; } = "Pending";

    [Required]
    [Column(TypeName = "decimal(18, 2)")]
    public decimal Subtotal { get; set; }

    [Required]
    [Column(TypeName = "decimal(18, 2)")]
    public decimal Discount { get; set; } = 0; 

    [Required]
    [Column(TypeName = "decimal(18, 2)")]
    public decimal Total { get; set; }

    [Required]
    [StringLength(3)]
    [Column(TypeName = "char(3)")] 
    public string Currency { get; set; } = "VND";

    [Required]
    [Precision(0)]
    public DateTimeOffset CreatedAt { get; set; } 

    [Precision(0)]
    public DateTimeOffset? PaidAt { get; set; }

    [StringLength(255)]
    public string? CancelReason { get; set; }

    // --- Navigation Properties ---

    [InverseProperty("Order")]
    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    [ForeignKey(nameof(UserId))]
    [InverseProperty("Orders")]
    public virtual AspNetUser User { get; set; } = null!;
}