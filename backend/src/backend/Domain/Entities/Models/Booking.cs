using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.Models;


public class Booking
{
    public long Id { get; set; }

    public long UserId { get; set; }

    public long VehicleId { get; set; }

    public long StartStationId { get; set; }

    public long? EndStationId { get; set; }

    public DateTimeOffset BookingTime { get; set; }

  
    public string Status { get; set; } = null!;
    public DateTimeOffset CreatedAt { get; set; }
    private Booking() { }

}
