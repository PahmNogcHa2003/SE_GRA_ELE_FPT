using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Quest
{
    public class QuestCreateDTO
    {
        public string Code { get; set; } = string.Empty;   // phải unique
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }

        /// <summary>
        /// "Distance" | "Trips" | "Duration"
        /// </summary>
        public string QuestType { get; set; } = "Distance";

        /// <summary>
        /// "Daily" | "Weekly" | "Monthly" | "OneTime"
        /// </summary>
        public string Scope { get; set; } = "Weekly";

        public decimal? TargetDistanceKm { get; set; }
        public int? TargetTrips { get; set; }
        public int? TargetDurationMinutes { get; set; }
        public decimal PromoReward { get; set; }
        public DateTimeOffset StartAt { get; set; }
        public DateTimeOffset EndAt { get; set; }

        public string Status { get; set; } = "Active";
    }
    public class QuestUpdateDTO : QuestCreateDTO
    {

    }
}
