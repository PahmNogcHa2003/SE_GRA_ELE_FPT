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

    [StringLength(255)]
    public string? ImagePublicId { get; set; }

    public DateTimeOffset UpdatedAt { get; set; }

    [InverseProperty("Station")]
    public virtual ICollection<StationLog> StationLogs { get; set; } = new List<StationLog>();

    [InverseProperty("Station")]
    public virtual ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();

    // Phương thức logic nghiệp vụ: Kích hoạt lại trạm
    public void Activate()
    {
        if (IsActive) return; // Nếu đã active thì không làm gì
        IsActive = true;
    }

    // Phương thức logic nghiệp vụ: Ngừng kích hoạt trạm
    public void Deactivate()
    {
        if (!IsActive) return; // Nếu đã inactive thì không làm gì
        IsActive = false;
    }

    // Phương thức logic nghiệp vụ: Cập nhật sức chứa
    public void UpdateCapacity(int newCapacity)
    {
        if (newCapacity <= 0)
        {
            throw new ArgumentException("Capacity must be greater than zero.", nameof(newCapacity));
        }
        Capacity = newCapacity;
    }
}