using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Domain.Entities;

[Table("Booking")]
public class Booking : BaseEntity<long>
{
    [Required]
    public long UserId { get; set; }

    [Required]
    public long VehicleId { get; set; }

    [Required]
    public long StartStationId { get; set; }

    public long? EndStationId { get; set; }

    [Required]
    [Precision(0)]
    public DateTimeOffset BookingTime { get; set; }

    [Required]
    [StringLength(20)]
    [Unicode(false)]
    public string Status { get; set; } = "Reserved";

    [Required]
    [Precision(0)]
    public DateTimeOffset CreatedAt { get; set; }

    [ForeignKey(nameof(UserId))]
    [InverseProperty(nameof(AspNetUser.Bookings))]
    public AspNetUser User { get; set; } = null!;

    [ForeignKey(nameof(VehicleId))]
    [InverseProperty(nameof(Vehicle.Bookings))]
    public Vehicle Vehicle { get; set; } = null!;

    [InverseProperty(nameof(BookingTicket.Booking))]
    public ICollection<BookingTicket> BookingTickets { get; set; } = new List<BookingTicket>();

    [InverseProperty(nameof(Rental.Booking))]
    public ICollection<Rental> Rentals { get; set; } = new List<Rental>();

}

