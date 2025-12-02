namespace Application.DTOs.Quest
{
    public class QuestDTO
    {
        public long Id { get; set; }
        public string Code { get; set; } = string.Empty;
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
        public DateTimeOffset? UpdatedAt { get; set; }
        public string Status { get; set; } = "Active";

        // ==== Progress của user hiện tại ====
        public decimal CurrentDistanceKm { get; set; }
        public int CurrentTrips { get; set; }
        public int CurrentDurationMinutes { get; set; }

        public bool IsCompleted { get; set; }
        public DateTimeOffset? CompletedAt { get; set; }
        public DateTimeOffset? RewardClaimedAt { get; set; }

        /// <summary>
        /// % hoàn thành (0–100)
        /// </summary>
        public decimal ProgressPercent { get; set; }
    }
}
