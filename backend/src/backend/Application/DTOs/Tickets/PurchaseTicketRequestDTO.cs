using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Tickets
{
    public class PurchaseTicketRequestDTO
    {
        [Required]
        public long UserId { get; set; } 
        [Required]
        public long PlanPriceId { get; set; } 
    }
}
