using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Models;

[Table("CategoriesVehicle")]
public partial class CategoriesVehicle
{
    [Key]
    public long Id { get; set; }

    [StringLength(100)]
    public string? Name { get; set; }

    [StringLength(255)]
    public string? Description { get; set; }

    public bool IsActive { get; set; }

    [InverseProperty("Category")]
    public virtual ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
}
