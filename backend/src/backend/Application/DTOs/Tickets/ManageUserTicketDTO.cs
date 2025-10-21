using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Tickets
{
    public class ManageUserTicketDTO
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public string UserEmail { get; set; } = string.Empty; 
        public long PlanPriceId { get; set; }
        public string PlanName { get; set; } = string.Empty; 

        public string? SerialCode { get; set; }
        public decimal? PurchasedPrice { get; set; }
        public string Status { get; set; } = string.Empty;

        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? ActivatedAt { get; set; }
        public DateTimeOffset? ExpiresAt { get; set; }
        public int? RemainingMinutes { get; set; }
    }
}
