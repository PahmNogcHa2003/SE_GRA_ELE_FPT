using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class RentalDTO 
    {
        [Required]
        public long UserId { get; set; }

        [Required]
        public long VehicleId { get; set; }

        [Required]
        public long StartStationId { get; set; }

        public long? EndStationId { get; set; }

        [Required]
        [Precision(0)]
        public DateTimeOffset StartTime { get; set; }

        [Precision(0)]
        public DateTimeOffset? EndTime { get; set; }

        [Required]
        [StringLength(20)]
        [Unicode(false)]
        public string Status { get; set; } = "Ongoing";
        [Required]
        [Precision(0)]
        public DateTimeOffset CreatedAt { get; set; }
    }
}
