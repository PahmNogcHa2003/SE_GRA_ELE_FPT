namespace Domain.Entities.Models
{
    public class Rental : BaseEntity<long>
    {
        public long BookingId { get; private set; }
        public DateTimeOffset StartTime { get; private set; }
        public DateTimeOffset? EndTime { get; private set; }
        public decimal? Distance { get; private set; }
        public string Status { get; private set; } = RentalStatus.Ongoing;

        private Rental() { } // Cho EF/AutoMapper

        // Factory method: bắt đầu một rental mới
        public static Rental Start(long bookingId)
        {
            return new Rental
            {
                BookingId = bookingId,
                StartTime = DateTimeOffset.UtcNow,
                Status = RentalStatus.Ongoing
            };
        }

        // Business method: kết thúc chuyến đi
        public void End(decimal? distance = null)
        {
            if (EndTime != null)
                throw new InvalidOperationException("Rental has already ended.");
            if (Status == RentalStatus.Cancelled)
                throw new InvalidOperationException("Cannot complete a cancelled rental.");

            EndTime = DateTimeOffset.UtcNow;
            Distance = distance;
            Status = RentalStatus.Completed;
        }

        // Business method: hủy chuyến đi
        public void Cancel()
        {
            if (EndTime != null)
                throw new InvalidOperationException("Rental has already ended.");
            if (Status == RentalStatus.Completed)
                throw new InvalidOperationException("Cannot cancel a completed rental.");

            EndTime = DateTimeOffset.UtcNow;
            Status = RentalStatus.Cancelled;
        }
    }

    // Tách trạng thái ra riêng
    public static class RentalStatus
    {
        public const string Ongoing = "Ongoing";
        public const string Completed = "Completed";
        public const string Cancelled = "Cancelled";
    }
}
