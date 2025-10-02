using System;
using System.Collections.Generic;

namespace Domain.Entities.Models
{
    public class Vehicle : BaseEntity<long> 
    {
        public long? CategoryId { get; set; }
        public string BikeCode { get; set; } = null!;
        public int? BatteryLevel { get; set; }
        public bool? ChargingStatus { get; set; }
        public string Status { get; set; } = null!;
        public long? StationId { get; set; }
        public DateTimeOffset CreatedAt { get; set; }

        public virtual CategoriesVehicle? Category { get; set; }
        public virtual Station? Station { get; set; }
        public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
        public virtual ICollection<VehicleUsageLog> VehicleUsageLogs { get; set; } = new List<VehicleUsageLog>();

        private Vehicle() { }
    }
}
