using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Domain.Entities;

[Index(nameof(BookingId), Name = "UQ_Rentals_Booking", IsUnique = true)]
public partial class Rental : BaseEntity<long>
{
    [Required]
    public long BookingId { get; set; }

    [Required]
    public long StartStationId { get; set; }

    public long? EndStationId { get; set; }

    [Required]
    [Precision(0)]
    public DateTimeOffset StartTime { get; set; }

    [Precision(0)]
    public DateTimeOffset? EndTime { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? TotalCost { get; set; }

    [Required]
    [StringLength(20)]
    [Unicode(false)]
    public string Status { get; set; } = "Ongoing";

    [ForeignKey(nameof(BookingId))]
    [InverseProperty("Rentals")]
    public virtual Booking Booking { get; set; } = null!;

}
