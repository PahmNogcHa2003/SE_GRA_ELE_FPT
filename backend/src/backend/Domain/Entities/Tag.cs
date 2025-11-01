using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

[Table("Tags")] // Khớp với tên bảng được tham chiếu trong Foreign Key
public partial class Tag : BaseEntity<long>
{
    [Required]
    [StringLength(50)]
    public string Name { get; set; } = null!;

    // Navigation property trỏ đến bảng nối TagNew
    [InverseProperty("Tag")]
    public virtual ICollection<TagNew> TagNews { get; set; } = new List<TagNew>();
}