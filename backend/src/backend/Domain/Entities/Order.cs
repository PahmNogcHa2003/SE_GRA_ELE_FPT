using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Domain.Entities;

[Table("Order")]
[Index("UserId", Name = "IX_Order_UserId")]
public partial class Order : BaseEntity<long>   
{
    public long UserId { get; set; }

    [StringLength(50)]
    public string OrderNo { get; set; } = null!;

    [StringLength(20)]
    public string Status { get; set; } = null!;

    public DateTimeOffset CreatedAt { get; set; }

    [InverseProperty("Order")]
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    [InverseProperty("Order")]
    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    [ForeignKey("UserId")]
    [InverseProperty("Orders")]
    public virtual AspNetUser User { get; set; } = null!;
}
