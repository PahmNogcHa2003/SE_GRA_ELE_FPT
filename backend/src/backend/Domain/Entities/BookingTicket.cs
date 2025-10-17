using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Domain.Entities;

[Table("BookingTicket")]
public class BookingTicket : BaseEntity<long>
{
    [Required]
    public long BookingId { get; set; }

    [Required]
    public long UserTicketId { get; set; }

    [Required]
    [Column(TypeName = "decimal(18, 2)")]
    public decimal PlanPrice { get; set; }

    [StringLength(50)]
    public string? VehicleType { get; set; }

    public int? UsedMinutes { get; set; }

    [Precision(0)]
    public DateTimeOffset? AppliedAt { get; set; }

    [ForeignKey(nameof(BookingId))]
    [InverseProperty(nameof(Booking.BookingTickets))]
    public Booking Booking { get; set; } = null!;

    [ForeignKey(nameof(UserTicketId))]
    [InverseProperty(nameof(UserTicket.BookingTickets))]
    public UserTicket UserTicket { get; set; } = null!;
}