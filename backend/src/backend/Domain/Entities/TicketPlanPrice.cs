using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Domain.Entities;

[Table("TicketPlanPrice")]
[Index("PlanId", Name = "IX_TicketPlanPrice_PlanId")]
public partial class TicketPlanPrice : BaseEntity<long> 
{
    public long PlanId { get; set; }

    [StringLength(50)]
    public string? VehicleType { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal Price { get; set; }

    [InverseProperty("PlanPrice")]
    public virtual ICollection<BookingTicket> BookingTickets { get; set; } = new List<BookingTicket>();

    [ForeignKey("PlanId")]
    [InverseProperty("TicketPlanPrices")]
    public virtual TicketPlan Plan { get; set; } = null!;

    [InverseProperty("PlanPrice")]
    public virtual ICollection<UserTicket> UserTickets { get; set; } = new List<UserTicket>();
}
