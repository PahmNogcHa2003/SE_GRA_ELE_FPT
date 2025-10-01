using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Models;

public partial class StationLog
{
    [Key]
    public long Id { get; set; }

    public long StationId { get; set; }

    [StringLength(50)]
    public string Action { get; set; } = null!;

    public DateTimeOffset Timestamp { get; set; }

    [ForeignKey("StationId")]
    [InverseProperty("StationLogs")]
    public virtual Station Station { get; set; } = null!;
}
