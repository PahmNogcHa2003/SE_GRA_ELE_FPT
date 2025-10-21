using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Tickets
{
    public class UpdateTicketPlanPriceDTO
    {
        public string? VehicleType { get; set; }
        public decimal? Price { get; set; }
        public int? DurationLimitMinutes { get; set; }
        public int? DailyFreeDurationMinutes { get; set; }
        public int? ValidityDays { get; set; }
        public decimal? OverageFeePer15Min { get; set; }
        public bool? IsActive { get; set; }
        public DateTimeOffset? ValidFrom { get; set; }
        public DateTimeOffset? ValidTo { get; set; }
    }
}
