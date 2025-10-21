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
        public string? SerialCode { get; set; }
        public decimal? PurchasedPrice { get; set; }
        public string Status { get; set; } = "Ready";
        public DateTimeOffset? ActivatedAt { get; set; }
        public DateTimeOffset? ExpiresAt { get; set; }
        public int? RemainingMinutes { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }
}
