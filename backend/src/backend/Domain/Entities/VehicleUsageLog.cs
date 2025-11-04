using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Domain.Entities;

[Table("VehicleUsageLogs")]
public partial class VehicleUsageLog : BaseEntity<long>
{
    [Required]
    public long VehicleId { get; set; }

    public long? UserId { get; set; }

    [StringLength(50)]
    public string? ChangeType { get; set; }

    [StringLength(255)]
    public string? Note { get; set; }

    [Required]
    [Precision(0)]
    public DateTimeOffset CreatedAt { get; set; }

    [ForeignKey(nameof(VehicleId))]
    [InverseProperty(nameof(Vehicle.VehicleUsageLogs))]
    public virtual Vehicle Vehicle { get; set; } = null!;

    [ForeignKey(nameof(UserId))]
    public virtual AspNetUser? User { get; set; }
}
