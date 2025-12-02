using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Rental
{
    public class CreateRentalDTO
    {
        [Required]
        public long VehicleId { get; set; }

        [Required]
        public long UserTicketId { get; set; }

        [Required]
        [Precision(0)]
        public DateTimeOffset StartTime { get; set; }

    }
}
