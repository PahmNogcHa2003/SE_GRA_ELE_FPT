using System;

namespace Domain.Entities.Models
{
    public class Booking : BaseEntity<long>
    {
        public long UserId { get; private set; }
        public long VehicleId { get; private set; }
        public long StartStationId { get; private set; }
        public long? EndStationId { get; private set; }
        public DateTimeOffset BookingTime { get; private set; }
        public string Status { get; private set; } = null!;
        public DateTimeOffset CreatedAt { get; private set; }

        // Factory method: tạo Booking mới
        public static Booking Create(long userId, long vehicleId, long startStationId, string status)
        {
            return new Booking
            {
                UserId = userId,
                VehicleId = vehicleId,
                StartStationId = startStationId,
                BookingTime = DateTimeOffset.UtcNow,
                Status = status,
                CreatedAt = DateTimeOffset.UtcNow
            };
        }

        // Domain method: kết thúc booking
        public void EndBooking(long endStationId)
        {
            EndStationId = endStationId;
            Status = "Completed";
        }

        // Private constructor cho EF Core
        private Booking() { }
    }
}
