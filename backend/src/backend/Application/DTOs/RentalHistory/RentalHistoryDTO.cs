using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Application.DTOs.RentalHistory
{
    public class RentalHistoryDTO
    {
        public long RentalId { get; set; }
        // Thời gian
        public DateTimeOffset StartTimeUtc { get; set; }
        public DateTimeOffset? EndTimeUtc { get; set; }
        public DateTimeOffset StartTimeVn { get; set; }
        public DateTimeOffset? EndTimeVn { get; set; }

        // Trạm & xe
        public string? StartStationName { get; set; }
        public string? EndStationName { get; set; }
        public string? VehicleCode { get; set; }
        public string? VehicleType { get; set; }      // Bike / EBike

        // Vé sử dụng
        public long? UserTicketId { get; set; }
        public string? TicketPlanName { get; set; }   // VD: "Vé tháng 79K"
        public string? TicketType { get; set; }       // Day / Month / Trip
        public string? TicketVehicleType { get; set; }

        // Thời lượng & quãng đường
        public int? DurationMinutes { get; set; }
        public double? DistanceKm { get; set; }
        public decimal? Co2SavedKg { get; set; }
        public decimal? CaloriesBurned { get; set; }

        // Overtime
        public int? OverusedMinutes { get; set; }
        public decimal? OverusedFee { get; set; }
        public bool IsOvertime => OverusedMinutes.HasValue && OverusedMinutes > 0;

        // Một số info khác
        public string Status { get; set; } = string.Empty; // Rental.Status
    }

}
