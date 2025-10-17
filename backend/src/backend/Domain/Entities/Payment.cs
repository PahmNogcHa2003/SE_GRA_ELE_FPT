using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Domain.Entities;

[Table("Payment")]
[Index(nameof(OrderId), Name = "IX_Payment_OrderId")]
// SỬA 1: Thêm composite unique index
[Index(nameof(Provider), nameof(ProviderTxnRef), IsUnique = true, Name = "UQ_Payment_ProviderTxnRef")]
public partial class Payment : BaseEntity<long>
{
    [Required]
    public long OrderId { get; set; }

    [Required]
    [StringLength(30)]
    [Unicode(false)]
    public string Provider { get; set; } = null!;

    [Required]
    [Column(TypeName = "decimal(18, 2)")]
    public decimal Amount { get; set; }

    [Required]
    [StringLength(3)]
    [Column(TypeName = "char(3)")] 
    public string Currency { get; set; } = "VND";

    [Required]
    [StringLength(20)]
    [Unicode(false)] 
    public string Status { get; set; } = "Pending"; 

    [Required]
    [StringLength(100)]
    public string ProviderTxnRef { get; set; } = null!;

    [StringLength(1024)]
    public string? CheckoutUrl { get; set; }

    [StringLength(100)]
    public string? GatewayTxnId { get; set; }

    [StringLength(100)]
    public string? IdempotencyKey { get; set; }

    public string? RawRequest { get; set; }

    public string? RawResponse { get; set; }

    [StringLength(255)]
    public string? FailureReason { get; set; }

    [Required]
    [Precision(0)]
    public DateTimeOffset CreatedAt { get; set; } 

    [Precision(0)]
    public DateTimeOffset? PaidAt { get; set; }

    // --- Navigation Property ---

    [ForeignKey("OrderId")]
    [InverseProperty("Payments")]
    public virtual Order Order { get; set; } = null!;
}