using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Tickets
{
    public class UserTicketPlanDTO
    {
        public long Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public List<UserTicketPlanPriceDTO> Prices { get; set; } = new List<UserTicketPlanPriceDTO>();
    }
}
