using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Domain.Entities;

[Table("CategoriesVehicle")]
public partial class CategoriesVehicle : BaseEntity<long>
{
    [StringLength(100)]
    public string? Name { get; set; }

    [StringLength(255)]
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;

    [InverseProperty("Category")]
    public virtual ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();

    // Constructor bắt buộc: Đảm bảo Name được set hợp lệ khi tạo
    public CategoriesVehicle(string name)
    {
        UpdateName(name);
    }

    // Constructor mặc định (cần cho EF Core và Unit Test Object Initializer)
    public CategoriesVehicle() { }

    // Phương thức logic nghiệp vụ: Cập nhật tên danh mục
    public void UpdateName(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
        {
            throw new ArgumentException("Category Name cannot be empty.", nameof(newName));
        }
        if (newName.Length > 100)
        {
            throw new ArgumentException("Category Name cannot exceed 100 characters.", nameof(newName));
        }

        Name = newName;
    }

    // Phương thức logic nghiệp vụ: Kích hoạt danh mục
    public void Activate()
    {
        IsActive = true;
    }

    // Phương thức logic nghiệp vụ: Vô hiệu hóa danh mục
    public void Deactivate()
    {
        IsActive = false;
    }
}