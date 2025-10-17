using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Domain.Entities;

[Table("StationLogs")]
[Index(nameof(StationId), Name = "IX_StationLogs_StationId")]
public partial class StationLog : BaseEntity<long>
{
    [Required]
    public long StationId { get; set; }

    [StringLength(50)]
    public string? ChangeType { get; set; }

    public int? Quantity { get; set; }

    [Required]
    [Precision(0)]
    public DateTimeOffset CreatedAt { get; set; }

    [ForeignKey(nameof(StationId))]
    [InverseProperty("StationLogs")]
    public virtual Station Station { get; set; } = null!;
}