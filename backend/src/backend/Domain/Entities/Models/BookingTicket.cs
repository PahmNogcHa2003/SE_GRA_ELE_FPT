namespace Domain.Entities.Models
{
    public class BookingTicket : BaseEntity<long>
    {
        public long BookingId { get; private set; }
        public long UserTicketId { get; private set; }
        public long PlanPriceId { get; private set; }
        public string? VehicleType { get; private set; }
        public int? UsedMinutes { get; private set; }
        public DateTimeOffset? AppliedAt { get; private set; }

        // Business method: đánh dấu ticket đã được dùng
        public void MarkAsUsed(int usedMinutes, DateTimeOffset appliedAt)
        {
            if (UsedMinutes != null)
                throw new InvalidOperationException("Ticket đã được sử dụng.");

            UsedMinutes = usedMinutes;
            AppliedAt = appliedAt;
        }

        // Factory method
        public static BookingTicket Create(long bookingId, long userTicketId, long planPriceId, string? vehicleType)
        {
            return new BookingTicket
            {
                BookingId = bookingId,
                UserTicketId = userTicketId,
                PlanPriceId = planPriceId,
                VehicleType = vehicleType
            };
        }

        // Private constructor cho EF
        private BookingTicket() { }
    }
}
