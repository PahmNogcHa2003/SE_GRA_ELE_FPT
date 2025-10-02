using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Domain.Entities;

[Table("OrderItem")]
[Index("OrderId", Name = "IX_OrderItem_OrderId")]
public partial class OrderItem : BaseEntity<long>
{ 
    public long OrderId { get; set; }

    [StringLength(150)]
    public string? ProductName { get; set; }

    public int Quantity { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal Price { get; set; }

    [ForeignKey("OrderId")]
    [InverseProperty("OrderItems")]
    public virtual Order Order { get; set; } = null!;
}
