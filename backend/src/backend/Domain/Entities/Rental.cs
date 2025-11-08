using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Domain.Entities;

[Table("Rental")]
public class Rental : BaseEntity<long>
{
    [Required]
    public long UserId { get; set; }

    [Required]
    public long VehicleId { get; set; }

    [Required]
    public long StartStationId { get; set; }

    public long? EndStationId { get; set; }

    [Required]
    [Precision(0)]
    public DateTimeOffset StartTime { get; set; }

    [Precision(0)]
    public DateTimeOffset? EndTime { get; set; }

    [Required]
    [StringLength(20)]
    [Unicode(false)]
    public string Status { get; set; } = "Ongoing";
    [Required]
    [Precision(0)]
    public DateTimeOffset CreatedAt { get; set; }

    [ForeignKey(nameof(UserId))]
    [InverseProperty(nameof(AspNetUser.Rentals))]
    public AspNetUser User { get; set; } = null!;

    [ForeignKey(nameof(VehicleId))]
    [InverseProperty(nameof(Vehicle.Rentals))]
    public Vehicle Vehicle { get; set; } = null!;

    [InverseProperty(nameof(BookingTicket.Rental))]
    public ICollection<BookingTicket> BookingTickets { get; set; } = new List<BookingTicket>();

    public void EndRental(DateTimeOffset endTime, long endStationId)
    {
        if (endTime < StartTime)
            throw new InvalidOperationException("End time cannot be before start time.");

        if (Status == RentalStatus.End)
            throw new InvalidOperationException("Rental already end.");

        EndTime = endTime;
        EndStationId = endStationId;
        Status = RentalStatus.End;
    }
}

