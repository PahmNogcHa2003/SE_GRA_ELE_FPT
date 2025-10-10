using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Domain.Entities;

[Table("BookingTicket")]
[Microsoft.EntityFrameworkCore.Index(nameof(BookingId), Name = "IX_BookingTicket_BookingId")]
[Microsoft.EntityFrameworkCore.Index(nameof(PlanPriceId), Name = "IX_BookingTicket_PlanPriceId")]
[Microsoft.EntityFrameworkCore.Index(nameof(UserTicketId), Name = "IX_BookingTicket_UserTicketId")]
public class BookingTicket : BaseEntity<long>
{
    [Required]
    public long BookingId { get; set; }

    [Required]
    public long UserTicketId { get; set; }

    [Required]
    public long PlanPriceId { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? VehicleType { get; set; }

    public int? UsedMinutes { get; set; }

    [Precision(0)]
    public DateTimeOffset? AppliedAt { get; set; }

    // 🔗 Navigation properties

    [ForeignKey(nameof(BookingId))]
    [InverseProperty(nameof(Booking.BookingTickets))]
    public Booking Booking { get; set; } = null!;

    [ForeignKey(nameof(PlanPriceId))]
    [InverseProperty(nameof(TicketPlanPrice.BookingTickets))]
    public TicketPlanPrice PlanPrice { get; set; } = null!;

    [ForeignKey(nameof(UserTicketId))]
    [InverseProperty(nameof(UserTicket.BookingTickets))]
    public UserTicket UserTicket { get; set; } = null!;
}
