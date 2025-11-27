using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.RentalHistory
{
    public class LeaderboardEntryDTO
    {
        public long UserId { get; set; }
        public string? FullName { get; set; }
        public string? AvatarUrl { get; set; }
        public decimal TotalDistanceKm { get; set; }
        public int TotalDurationMinutes { get; set; }
        public int TotalTrips { get; set; }
        public int Rank { get; set; }
    }
}
