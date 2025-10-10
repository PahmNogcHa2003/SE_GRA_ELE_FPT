using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Domain.Entities;

[Microsoft.EntityFrameworkCore.Index(nameof(StationId), Name = "IX_StationLogs_StationId")]
public partial class StationLog : BaseEntity<long>
{

    public long StationId { get; set; }

    [StringLength(50)]
    public string Action { get; set; } = null!;

    public DateTimeOffset Timestamp { get; set; }

    [ForeignKey("StationId")]
    [InverseProperty("StationLogs")]
    public virtual Station Station { get; set; } = null!;
}
