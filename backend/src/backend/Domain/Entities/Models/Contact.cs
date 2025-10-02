namespace Domain.Entities.Models
{
    public class Contact : BaseEntity<long>
    {
        public long UserId { get; private set; }
        public string Message { get; private set; } = null!;
        public DateTimeOffset CreatedAt { get; private set; }
        public DateTimeOffset? UpdatedAt { get; private set; }

        // Factory method: tạo Contact mới
        public static Contact Create(long userId, string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentException("Message cannot be empty.");

            return new Contact
            {
                UserId = userId,
                Message = message,
                CreatedAt = DateTimeOffset.UtcNow
            };
        }

        // Business method: update message
        public void UpdateMessage(string newMessage)
        {
            if (string.IsNullOrWhiteSpace(newMessage))
                throw new ArgumentException("Message cannot be empty.");

            Message = newMessage;
            UpdatedAt = DateTimeOffset.UtcNow;
        }

        private Contact() { } // constructor rỗng cho EF/AutoMapper
    }
}
