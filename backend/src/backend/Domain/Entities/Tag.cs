// File: Domain.Entities/Tag.cs

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

[Table("Tags")]
public partial class Tag : BaseEntity<long>
{
    [Required]
    [StringLength(50)]
    public string Name { get; set; } = null!;

    [InverseProperty("Tag")]
    public virtual ICollection<TagNew> TagNews { get; set; } = new List<TagNew>();

    // Constructor mặc định (cần cho EF Core, nên để là private hoặc protected nếu có thể)
    // Tạm thời để public để không xung đột với constructor bên dưới
    public Tag() { }

    // Constructor bắt buộc: Đảm bảo Name được set hợp lệ
    public Tag(string name)
    {
        UpdateName(name); // Tái sử dụng logic kiểm tra
    }

    // Phương thức logic nghiệp vụ: Cập nhật tên thẻ
    public void UpdateName(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
        {
            throw new ArgumentException("Tag Name cannot be empty.", nameof(newName));
        }
        if (newName.Length > 50)
        {
            throw new ArgumentException("Tag Name cannot exceed 50 characters.", nameof(newName));
        }

        Name = newName;
    }
}