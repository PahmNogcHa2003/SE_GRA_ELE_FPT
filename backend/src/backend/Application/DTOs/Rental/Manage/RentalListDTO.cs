using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Rental.Manage
{
    public class RentalListDTO
    {
        public long Id { get; set; }

        // User
        public long UserId { get; set; }
        public string? UserFullName { get; set; }

        // Vehicle
        public string? BikeCode { get; set; }
        public string? VehicleType { get; set; }

        // Stations
        public string? StartStationName { get; set; }
        public string? EndStationName { get; set; }

        // Time
        public DateTimeOffset StartTimeUtc { get; set; }
        public DateTimeOffset? EndTimeUtc { get; set; }
        public int? DurationMinutes { get; set; }

        // Important Fee
        public decimal? OverusedFee { get; set; }

        public string Status { get; set; } = string.Empty;
    }
}
