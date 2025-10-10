using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Domain.Entities;

[Table("Booking")]
[Microsoft.EntityFrameworkCore.Index(nameof(EndStationId), Name = "IX_Booking_EndStationId")]
[Microsoft.EntityFrameworkCore.Index(nameof(StartStationId), Name = "IX_Booking_StartStationId")]
[Microsoft.EntityFrameworkCore.Index(nameof(UserId), Name = "IX_Booking_UserId")]
[Microsoft.EntityFrameworkCore.Index(nameof(VehicleId), Name = "IX_Booking_VehicleId")]
public class Booking : BaseEntity<long>
{
    [Required]
    public long UserId { get; set; }

    [Required]
    public long VehicleId { get; set; }

    [Required]
    public long StartStationId { get; set; }

    public long? EndStationId { get; set; }

    [Precision(0)]
    public DateTimeOffset BookingTime { get; set; }

    [Required]
    [StringLength(20)]
    [Unicode(false)]
    public string Status { get; set; } = string.Empty;

    [Precision(0)]
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    // 🧭 Navigation Properties

    [ForeignKey(nameof(EndStationId))]
    [InverseProperty(nameof(Station.BookingEndStations))]
    public Station? EndStation { get; set; }

    [ForeignKey(nameof(StartStationId))]
    [InverseProperty(nameof(Station.BookingStartStations))]
    public Station StartStation { get; set; } = null!;

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

    [InverseProperty(nameof(VehicleUsageLog.Booking))]
    public ICollection<VehicleUsageLog> VehicleUsageLogs { get; set; } = new List<VehicleUsageLog>();
}
