using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Models;

public partial class Rental
{
    [Key]
    public long Id { get; set; }

    public long BookingId { get; set; }

    public DateTimeOffset StartTime { get; set; }

    public DateTimeOffset? EndTime { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal? Distance { get; set; }

    [StringLength(20)]
    public string Status { get; set; } = null!;

    [ForeignKey("BookingId")]
    [InverseProperty("Rentals")]
    public virtual Booking Booking { get; set; } = null!;
}
