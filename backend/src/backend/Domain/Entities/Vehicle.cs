using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Domain.Entities;

[Table("Vehicle")]
[Microsoft.EntityFrameworkCore.Index(nameof(BikeCode), Name = "IX_Vehicle_BikeCode", IsUnique = true)]
[Microsoft.EntityFrameworkCore.Index(nameof(CategoryId), Name = "IX_Vehicle_CategoryId")]
[Microsoft.EntityFrameworkCore.Index(nameof(StationId), Name = "IX_Vehicle_StationId")]
public partial class Vehicle : BaseEntity<long>
{
    public long? CategoryId { get; set; }

    [StringLength(50)]
    public string BikeCode { get; set; } = null!;

    public int? BatteryLevel { get; set; }

    public bool? ChargingStatus { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string Status { get; set; } = null!;

    public long? StationId { get; set; }

    [Precision(0)]
    public DateTimeOffset CreatedAt { get; set; }

    [InverseProperty(nameof(Booking.Vehicle))]
    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    [ForeignKey(nameof(CategoryId))]
    [InverseProperty(nameof(CategoriesVehicle.Vehicles))]
    public virtual CategoriesVehicle? Category { get; set; }

    [ForeignKey(nameof(StationId))]
    [InverseProperty(nameof(Station.Vehicles))]
    public virtual Station? Station { get; set; }

    [InverseProperty(nameof(VehicleUsageLog.Vehicle))]
    public virtual ICollection<VehicleUsageLog> VehicleUsageLogs { get; set; } = new List<VehicleUsageLog>();
}
