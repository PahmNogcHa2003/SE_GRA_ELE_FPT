using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Rental.Manage
{
    public class RentalHistoryItemDTO
    {
        public long Id { get; set; }
        public string ActionType { get; set; } = string.Empty; 
        public DateTimeOffset TimestampUtc { get; set; }
        public string? Description { get; set; }
        public double? DistanceKm { get; set; }
    }
}
