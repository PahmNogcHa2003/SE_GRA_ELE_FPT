using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Domain.Entities;

[Table("Vehicle")]
public partial class Vehicle : BaseEntity<long>
{
    public long? CategoryId { get; set; }

    [Required]
    [StringLength(50)]
    public string BikeCode { get; set; } = null!;

    public int? BatteryLevel { get; set; }

    public bool? ChargingStatus { get; set; }

    [Required]
    [StringLength(20)]
    [Unicode(false)]
    public string Status { get; set; } = "Available";

    public long? StationId { get; set; }

    [Required]
    [Precision(0)]
    public DateTimeOffset CreatedAt { get; set; }

    [InverseProperty(nameof(Rental.Vehicle))]
    public virtual ICollection<Rental> Bookings { get; set; } = new List<Rental>();

    [ForeignKey(nameof(CategoryId))]
    [InverseProperty(nameof(CategoriesVehicle.Vehicles))]
    public virtual CategoriesVehicle? Category { get; set; }

    [ForeignKey(nameof(StationId))]
    [InverseProperty(nameof(Station.Vehicles))]
    public virtual Station? Station { get; set; }

    [InverseProperty(nameof(VehicleUsageLog.Vehicle))]
    public virtual ICollection<VehicleUsageLog> VehicleUsageLogs { get; set; } = new List<VehicleUsageLog>();
}
