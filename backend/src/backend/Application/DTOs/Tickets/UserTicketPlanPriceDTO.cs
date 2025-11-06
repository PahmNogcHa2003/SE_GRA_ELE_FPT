using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Tickets
{
    public enum ActivationModeDTO { IMMEDIATE, ON_FIRST_USE }
    public class UserTicketPlanPriceDTO
    {
        public long Id { get; set; }
        public string? VehicleType { get; set; }
        public decimal Price { get; set; }
        public int? ValidityDays { get; set; }
        public int? DurationLimitMinutes { get; set; }
        public int? DailyFreeDurationMinutes { get; set; }
        public decimal? OverageFeePer15Min { get; set; }
        public ActivationModeDTO ActivationMode { get; set; }
        public int? ActivationWindowDays { get; set; }
    }
}
