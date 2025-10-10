using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Domain.Entities;

[Microsoft.EntityFrameworkCore.Index(nameof(BookingId), Name = "IX_VehicleUsageLogs_BookingId")]
[Microsoft.EntityFrameworkCore.Index(nameof(VehicleId), Name = "IX_VehicleUsageLogs_VehicleId")]
public partial class VehicleUsageLog : BaseEntity<long>
{
    public long VehicleId { get; set; }

    public long BookingId { get; set; }

    [StringLength(20)]
    public string Status { get; set; } = null!;

    public DateTimeOffset Timestamp { get; set; }

    [ForeignKey(nameof(BookingId))]
    [InverseProperty(nameof(Booking.VehicleUsageLogs))]
    public virtual Booking Booking { get; set; } = null!;

    [ForeignKey(nameof(VehicleId))]
    [InverseProperty(nameof(Vehicle.VehicleUsageLogs))]
    public virtual Vehicle Vehicle { get; set; } = null!;
}
