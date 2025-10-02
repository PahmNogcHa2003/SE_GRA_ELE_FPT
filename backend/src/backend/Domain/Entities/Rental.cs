using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Domain.Entities;

[Index("BookingId", Name = "IX_Rentals_BookingId")]
public partial class Rental : BaseEntity<long>
{

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
