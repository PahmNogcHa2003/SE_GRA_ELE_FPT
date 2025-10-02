namespace Domain.Entities.Models
{
    public class StationLog : BaseEntity<long>
    {
        public long StationId { get; private set; }
        public string Action { get; private set; } = null!;
        public DateTimeOffset Timestamp { get; private set; }

        // Navigation property (optional cho ORM, không dùng trong domain logic)
        public Station? Station { get; private set; }

        private StationLog() { } // Cho EF/AutoMapper

        // Factory method: ghi log
        public static StationLog Create(long stationId, string action)
        {
            if (string.IsNullOrWhiteSpace(action))
                throw new ArgumentException("Action cannot be empty.", nameof(action));

            return new StationLog
            {
                StationId = stationId,
                Action = action,
                Timestamp = DateTimeOffset.UtcNow
            };
        }
    }

    // Gợi ý: để tránh hardcode string, bạn có thể định nghĩa các action sẵn
    public static class StationActions
    {
        public const string VehicleArrived = "VehicleArrived";
        public const string VehicleDeparted = "VehicleDeparted";
        public const string Maintenance = "Maintenance";
        public const string Incident = "Incident";
    }
}
