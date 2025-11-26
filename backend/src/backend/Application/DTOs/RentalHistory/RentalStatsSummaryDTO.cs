using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.RentalHistory
{
    public class RentalStatsSummaryDTO
    {
        public decimal TotalDistanceKm { get; set; }
        public int TotalTrips { get; set; }
        public int TotalDurationMinutes { get; set; }
        public decimal TotalCo2SavedKg { get; set; }
        public decimal TotalCaloriesBurned { get; set; }
    }
}
