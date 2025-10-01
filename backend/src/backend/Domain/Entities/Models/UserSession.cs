namespace Domain.Entities.Models
{
    public class UserSession
    {
        public long Id { get; private set; }
        public long UserId { get; private set; }
        public string? SessionToken { get; private set; }
        public DateTimeOffset? Expiry { get; private set; }

        private UserSession() { } // EF/AutoMapper cần

        // Factory method
        public static UserSession Create(long userId, string? sessionToken, DateTimeOffset? expiry)
        {
            if (userId <= 0)
                throw new ArgumentException("Invalid UserId", nameof(userId));

            return new UserSession
            {
                UserId = userId,
                SessionToken = sessionToken,
                Expiry = expiry
            };
        }

        // Business methods
        public void RefreshSession(string newToken, DateTimeOffset newExpiry)
        {
            SessionToken = newToken;
            Expiry = newExpiry;
        }

        public bool IsExpired()
        {
            return Expiry.HasValue && Expiry.Value < DateTimeOffset.UtcNow;
        }
    }
}
