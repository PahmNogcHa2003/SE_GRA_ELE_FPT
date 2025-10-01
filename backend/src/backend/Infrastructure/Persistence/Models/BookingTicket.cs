using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Models;

[Table("BookingTicket")]
public partial class BookingTicket
{
    [Key]
    public long Id { get; set; }

    public long BookingId { get; set; }

    public long UserTicketId { get; set; }

    public long PlanPriceId { get; set; }

    [StringLength(50)]
    public string? VehicleType { get; set; }

    public int? UsedMinutes { get; set; }

    [Precision(0)]
    public DateTimeOffset? AppliedAt { get; set; }

    [ForeignKey("BookingId")]
    [InverseProperty("BookingTickets")]
    public virtual Booking Booking { get; set; } = null!;

    [ForeignKey("PlanPriceId")]
    [InverseProperty("BookingTickets")]
    public virtual TicketPlanPrice PlanPrice { get; set; } = null!;

    [ForeignKey("UserTicketId")]
    [InverseProperty("BookingTickets")]
    public virtual UserTicket UserTicket { get; set; } = null!;
}
