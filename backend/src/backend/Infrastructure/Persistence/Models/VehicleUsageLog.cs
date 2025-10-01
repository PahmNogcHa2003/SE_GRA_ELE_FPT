using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Models;

public partial class VehicleUsageLog
{
    [Key]
    public long Id { get; set; }

    public long VehicleId { get; set; }

    public long BookingId { get; set; }

    [StringLength(20)]
    public string Status { get; set; } = null!;

    public DateTimeOffset Timestamp { get; set; }

    [ForeignKey("BookingId")]
    [InverseProperty("VehicleUsageLogs")]
    public virtual Booking Booking { get; set; } = null!;

    [ForeignKey("VehicleId")]
    [InverseProperty("VehicleUsageLogs")]
    public virtual Vehicle Vehicle { get; set; } = null!;
}
