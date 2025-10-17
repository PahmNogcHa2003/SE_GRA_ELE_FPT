using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Domain.Entities;

namespace Domain.Entities;

[Table("TagNew")]
[Index(nameof(NewId), nameof(TagId), IsUnique = true, Name = "UQ_TagNew")]
public class TagNew : BaseEntity<long>
{
    public long NewId { get; set; }
    public long TagId { get; set; }

    // Navigation properties đến 2 bảng chính
    [ForeignKey(nameof(NewId))]
    public virtual News News { get; set; } = null!;

    [ForeignKey(nameof(TagId))]
    public virtual Tag Tag { get; set; } = null!;
}
