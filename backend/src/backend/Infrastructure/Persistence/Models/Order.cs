using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Models;

[Table("Order")]
public partial class Order
{
    [Key]
    public long Id { get; set; }

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
