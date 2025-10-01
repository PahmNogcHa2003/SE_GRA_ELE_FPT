using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Models;

[Table("TicketPlan")]
public partial class TicketPlan
{
    [Key]
    public long Id { get; set; }

    [StringLength(100)]
    public string Name { get; set; } = null!;

    [StringLength(255)]
    public string? Description { get; set; }

    public int DurationMinutes { get; set; }

    public bool IsActive { get; set; }

    [InverseProperty("Plan")]
    public virtual ICollection<TicketPlanPrice> TicketPlanPrices { get; set; } = new List<TicketPlanPrice>();
}
