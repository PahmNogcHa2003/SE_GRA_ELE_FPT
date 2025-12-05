using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Rental.Manage
{
    public class RentalDetailDTO
    {
        public long Id { get; set; }

        // User
        public long UserId { get; set; }
        public string? UserFullName { get; set; }
        public string? UserEmail { get; set; }
        public string? UserPhone { get; set; }

        // Vehicle
        public string? BikeCode { get; set; }
        public string? VehicleType { get; set; }

        // Station
        public string? StartStationName { get; set; }
        public string? EndStationName { get; set; }

        // Time
        public DateTimeOffset StartTimeUtc { get; set; }
        public DateTimeOffset? EndTimeUtc { get; set; }
        public int? DurationMinutes { get; set; }
        public double? DistanceKm { get; set; }

        // Ticket info
        public long? UserTicketId { get; set; }
        public string? TicketPlanName { get; set; }
        public string? TicketType { get; set; }
        public decimal? TicketPlanPrice { get; set; }

        // Overuse
        public int? OverusedMinutes { get; set; }
        public decimal? OverusedFee { get; set; }

        public string Status { get; set; } = string.Empty;
        public DateTimeOffset CreatedAt { get; set; }

        // Tiền nợ (nếu có)
        public long? OvertimeOrderId { get; set; }
        public string? OvertimeOrderNo { get; set; }
        public string? OvertimeOrderStatus { get; set; }
        public decimal? OvertimeDebtAmount { get; set; }
        public decimal? OvertimeDebtRemaining { get; set; }
        public string? OvertimeDebtStatus { get; set; }

        // Lịch sử RentalHistory
        public List<RentalHistoryItemDTO> History { get; set; } = new();
    }
}
