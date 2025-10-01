using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.Models;

public class BookingTicket
{
    public long Id { get; set; }

    public long BookingId { get; set; }

    public long UserTicketId { get; set; }

    public long PlanPriceId { get; set; }

    public string? VehicleType { get; set; }

    public int? UsedMinutes { get; set; }

    public DateTimeOffset? AppliedAt { get; set; }

    private BookingTicket() { }
    // Business method (ví dụ): đánh dấu ticket đã được dùng
    public void MarkAsUsed(int usedMinutes, DateTimeOffset appliedAt)
    {
        UsedMinutes = usedMinutes;
        AppliedAt = appliedAt;
    }
}
