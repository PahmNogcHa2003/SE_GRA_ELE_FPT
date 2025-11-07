using Application.DTOs.BaseDTO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Tickets
{
    public class TicketPlanDTO : BaseDTO<long>
    {
        [StringLength(50)]
        public string? Code { get; set; }

        [StringLength(50)]
        public string? Type { get; set; }

        [StringLength(150)]
        public string? Name { get; set; }

        [StringLength(255)]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
