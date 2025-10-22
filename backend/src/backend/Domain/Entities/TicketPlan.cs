using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Domain.Entities;

[Table("TicketPlan")]
public partial class TicketPlan : BaseEntity<long>
{
    [StringLength(50)]
    public string? Code { get; set; }

    [StringLength(50)]
    public string? Type { get; set; }

    [StringLength(150)]
    public string? Name { get; set; }

    [StringLength(255)]
    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;

    [Required]
    [Precision(0)]
    public DateTimeOffset CreatedAt { get; set; }

    [InverseProperty("Plan")]
    public virtual ICollection<TicketPlanPrice> TicketPlanPrices { get; set; } = new List<TicketPlanPrice>();
}