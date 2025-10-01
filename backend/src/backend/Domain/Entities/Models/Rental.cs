namespace Domain.Entities.Models
{
    public class Rental
    {
        public long Id { get; private set; }
        public long BookingId { get; private set; }
        public DateTimeOffset StartTime { get; private set; }
        public DateTimeOffset? EndTime { get; private set; }
        public decimal? Distance { get; private set; }
        public string Status { get; private set; } = "Ongoing";

        private Rental() { } // constructor rỗng cho EF/AutoMapper

        // Factory method: bắt đầu một rental mới
        public static Rental Start(long bookingId)
        {
            return new Rental
            {
                BookingId = bookingId,
                StartTime = DateTimeOffset.UtcNow,
                Status = "Ongoing"
            };
        }

        // Business method: kết thúc chuyến đi
        public void End(decimal? distance = null)
        {
            if (EndTime != null)
                throw new InvalidOperationException("Rental has already ended.");

            EndTime = DateTimeOffset.UtcNow;
            Distance = distance;
            Status = "Completed";
        }

        // Business method: hủy chuyến đi
        public void Cancel()
        {
            if (EndTime != null)
                throw new InvalidOperationException("Rental has already ended.");

            EndTime = DateTimeOffset.UtcNow;
            Status = "Cancelled";
        }
    }
}
