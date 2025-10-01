namespace Domain.Entities.Models
{
    public class StationLog
    {
        public long Id { get; private set; }
        public long StationId { get; private set; }
        public string Action { get; private set; } = null!;
        public DateTimeOffset Timestamp { get; private set; }

        // Navigation (nếu cần sử dụng trong domain logic)
        public Station? Station { get; private set; }

        private StationLog() { } // constructor rỗng cho EF/AutoMapper

        // Factory method
        public static StationLog Create(long stationId, string action, DateTimeOffset timestamp)
        {
            if (string.IsNullOrWhiteSpace(action))
                throw new ArgumentException("Action cannot be empty.", nameof(action));

            return new StationLog
            {
                StationId = stationId,
                Action = action,
                Timestamp = timestamp
            };
        }
    }
}
