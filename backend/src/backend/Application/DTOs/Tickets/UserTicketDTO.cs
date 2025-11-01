using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Tickets
{
    public class UserTicketDTO
    {
        public long Id { get; set; }
        public long PlanPriceId { get; set; }
        public string PlanName { get; set; } = string.Empty;
        public string VehicleType { get; set; } = "Bike";
        public string? SerialCode { get; set; }
        public decimal? PurchasedPrice { get; set; }
        public string Status { get; set; } = "Ready";
        public ActivationModeDTO ActivationMode { get; set; }

        public DateTimeOffset? ActivatedAt { get; set; }
        public DateTimeOffset? ValidFrom { get; set; }
        public DateTimeOffset? ValidTo { get; set; }
        public DateTimeOffset? ExpiresAt { get; set; }
        public DateTimeOffset? ActivationDeadline { get; set; }
        public int? RemainingMinutes { get; set; }
        public int? RemainingRides { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }
}
