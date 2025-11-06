using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.BookingTicket
{
    public class CreateBookingTicketDTO
    {
        [Required]
        public long RentalId { get; set; }

        [Required]
        public long UserTicketId { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal PlanPrice { get; set; }

        [StringLength(50)]
        public string? VehicleType { get; set; }

        public int? UsedMinutes { get; set; }
    }
}
