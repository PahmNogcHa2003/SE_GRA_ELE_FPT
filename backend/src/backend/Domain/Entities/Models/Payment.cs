namespace Domain.Entities.Models
{
    public class Payment
    {
        public long Id { get; private set; }
        public long OrderId { get; private set; }
        public decimal Amount { get; private set; }
        public string Method { get; private set; } = null!;
        public string Status { get; private set; } = "Pending";
        public DateTimeOffset CreatedAt { get; private set; }

        private Payment() { } // constructor rỗng cho EF/AutoMapper

        // Factory method: tạo payment mới
        public static Payment Create(long orderId, decimal amount, string method)
        {
            if (amount <= 0)
                throw new ArgumentException("Amount must be greater than zero.");
            if (string.IsNullOrWhiteSpace(method))
                throw new ArgumentException("Payment method cannot be empty.");

            return new Payment
            {
                OrderId = orderId,
                Amount = amount,
                Method = method,
                Status = "Pending",
                CreatedAt = DateTimeOffset.UtcNow
            };
        }

        // Business methods
        public void MarkAsCompleted()
        {
            Status = "Completed";
        }

        public void MarkAsFailed()
        {
            Status = "Failed";
        }
    }
}
