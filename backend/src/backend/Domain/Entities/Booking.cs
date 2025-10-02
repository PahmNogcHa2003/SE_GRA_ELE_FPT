using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Domain.Entities;

[Table("Booking")]
[Index("EndStationId", Name = "IX_Booking_EndStationId")]
[Index("StartStationId", Name = "IX_Booking_StartStationId")]
[Index("UserId", Name = "IX_Booking_UserId")]
[Index("VehicleId", Name = "IX_Booking_VehicleId")]
public partial class Booking : BaseEntity<long>
{

    public long UserId { get; set; }

    public long VehicleId { get; set; }

    public long StartStationId { get; set; }

    public long? EndStationId { get; set; }

    [Precision(0)]
    public DateTimeOffset BookingTime { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string Status { get; set; } = null!;

    [Precision(0)]
    public DateTimeOffset CreatedAt { get; set; }

    [InverseProperty("Booking")]
    public virtual ICollection<BookingTicket> BookingTickets { get; set; } = new List<BookingTicket>();

    [ForeignKey("EndStationId")]
    [InverseProperty("BookingEndStations")]
    public virtual Station? EndStation { get; set; }

    [InverseProperty("Booking")]
    public virtual ICollection<Rental> Rentals { get; set; } = new List<Rental>();

    [ForeignKey("StartStationId")]
    [InverseProperty("BookingStartStations")]
    public virtual Station StartStation { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("Bookings")]
    public virtual AspNetUser User { get; set; } = null!;

    [ForeignKey("VehicleId")]
    [InverseProperty("Bookings")]
    public virtual Vehicle Vehicle { get; set; } = null!;

    [InverseProperty("Booking")]
    public virtual ICollection<VehicleUsageLog> VehicleUsageLogs { get; set; } = new List<VehicleUsageLog>();
}
