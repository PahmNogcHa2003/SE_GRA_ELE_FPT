namespace Domain.Entities.Models
{
    public class UserTicket : BaseEntity<long>
    {
        public long UserId { get; private set; }
        public long PlanPriceId { get; private set; }
        public DateTimeOffset StartTime { get; private set; }
        public DateTimeOffset EndTime { get; private set; }
        public bool IsUsed { get; private set; }

        private UserTicket() { } // EF / mapping cần

        // Factory
        public static UserTicket Create(long userId, long planPriceId, DateTimeOffset startTime, DateTimeOffset endTime)
        {
            if (userId <= 0) throw new ArgumentException("Invalid UserId", nameof(userId));
            if (planPriceId <= 0) throw new ArgumentException("Invalid PlanPriceId", nameof(planPriceId));
            if (endTime <= startTime) throw new ArgumentException("EndTime must be after StartTime");

            return new UserTicket
            {
                UserId = userId,
                PlanPriceId = planPriceId,
                StartTime = startTime,
                EndTime = endTime,
                IsUsed = false
            };
        }

        // Business logic
        public void MarkAsUsed()
        {
            if (IsExpired())
                throw new InvalidOperationException("Cannot use an expired ticket");

            if (IsUsed)
                throw new InvalidOperationException("Ticket already used");

            IsUsed = true;
        }

        public bool IsExpired()
        {
            return EndTime < DateTimeOffset.UtcNow;
        }

        public bool IsActive()
        {
            return !IsUsed && !IsExpired();
        }
    }
}
