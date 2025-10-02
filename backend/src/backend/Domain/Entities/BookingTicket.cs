using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Domain.Entities;

[Table("BookingTicket")]
[Index("BookingId", Name = "IX_BookingTicket_BookingId")]
[Index("PlanPriceId", Name = "IX_BookingTicket_PlanPriceId")]
[Index("UserTicketId", Name = "IX_BookingTicket_UserTicketId")]
public partial class BookingTicket : BaseEntity<long>
{
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
