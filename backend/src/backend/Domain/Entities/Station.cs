using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Domain.Entities;

[Table("Station")]
public partial class Station : BaseEntity<long>
{
    [StringLength(150)]
    public string? Name { get; set; }

    [StringLength(255)]
    public string? Location { get; set; }

    public int? Capacity { get; set; }

    [Column(TypeName = "decimal(9, 6)")]
    public decimal? Lat { get; set; }

    [Column(TypeName = "decimal(9, 6)")]
    public decimal? Lng { get; set; }

    public bool IsActive { get; set; } = true;

    [StringLength(255)]
    public string? Image { get; set; }

    [InverseProperty("Station")]
    public virtual ICollection<StationLog> StationLogs { get; set; } = new List<StationLog>();

    [InverseProperty("Station")]
    public virtual ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
}