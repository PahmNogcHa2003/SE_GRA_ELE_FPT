namespace Domain.Entities.Models
{
    public class Contact
    {
        public long Id { get; private set; }
        public long UserId { get; private set; }
        public string? Message { get; private set; }
        public DateTimeOffset CreatedAt { get; private set; }

        private Contact() { } // constructor rỗng cho EF/AutoMapper

        // Factory method: tạo Contact mới
        public static Contact Create(long userId, string? message)
        {
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
        }
    }
}
