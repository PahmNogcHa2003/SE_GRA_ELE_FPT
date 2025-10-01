namespace Domain.Entities.Models
{
    public class UserDevice
    {
        public long Id { get; private set; }
        public long UserId { get; private set; }
        public string? DeviceId { get; private set; }
        public string? DeviceType { get; private set; }
        public DateTimeOffset? LastLogin { get; private set; }

        private UserDevice() { } // EF/AutoMapper cần constructor rỗng

        // Factory method
        public static UserDevice Create(long userId, string? deviceId, string? deviceType)
        {
            if (userId <= 0)
                throw new ArgumentException("Invalid UserId", nameof(userId));

            return new UserDevice
            {
                UserId = userId,
                DeviceId = deviceId,
                DeviceType = deviceType,
                LastLogin = DateTimeOffset.UtcNow
            };
        }

        // Business methods
        public void UpdateLastLogin()
        {
            LastLogin = DateTimeOffset.UtcNow;
        }

        public void UpdateDeviceInfo(string? newDeviceId, string? newDeviceType)
        {
            DeviceId = newDeviceId;
            DeviceType = newDeviceType;
        }
    }
}
