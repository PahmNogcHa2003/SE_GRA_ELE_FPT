using System;

namespace Domain.Entities.Models
{
    public class VehicleUsageLog : BaseEntity<long> 
    {
        public long VehicleId { get; set; }
        public long BookingId { get; set; }
        public string Status { get; set; } = null!;
        public DateTimeOffset Timestamp { get; set; }

        // Navigation properties
        public virtual Vehicle Vehicle { get; set; } = null!;
        public virtual Booking Booking { get; set; } = null!;

        private VehicleUsageLog() { }
    }
}
