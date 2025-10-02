using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Domain.Entities;

[Table("Vehicle")]
[Index("BikeCode", Name = "IX_Vehicle_BikeCode", IsUnique = true)]
[Index("CategoryId", Name = "IX_Vehicle_CategoryId")]
[Index("StationId", Name = "IX_Vehicle_StationId")]
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

    [InverseProperty("Vehicle")]
    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    [ForeignKey("CategoryId")]
    [InverseProperty("Vehicles")]
    public virtual CategoriesVehicle? Category { get; set; }

    [ForeignKey("StationId")]
    [InverseProperty("Vehicles")]
    public virtual Station? Station { get; set; }

    [InverseProperty("Vehicle")]
    public virtual ICollection<VehicleUsageLog> VehicleUsageLogs { get; set; } = new List<VehicleUsageLog>();
}
