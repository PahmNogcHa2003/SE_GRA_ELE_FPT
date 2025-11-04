using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Tickets
{
    public class CreateTicketPlanPriceDTO
    {
        [Required]
        public long PlanId { get; set; }               
        public string? VehicleType { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }
        public int? DurationLimitMinutes { get; set; }
        public int? DailyFreeDurationMinutes { get; set; }
        public int? ValidityDays { get; set; }
        public decimal? OverageFeePer15Min { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTimeOffset? ValidFrom { get; set; }
        public DateTimeOffset? ValidTo { get; set; }
    }
}
