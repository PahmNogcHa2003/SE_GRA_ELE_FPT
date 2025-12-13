using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Tickets
{
    public class PreviewTicketRequestDTO
    {
        public long PlanPriceId { get; set; }
        public string? VoucherCode { get; set; }
    }
    
}
