namespace Domain.Entities.Models
{
    public class Payment : BaseEntity<long>
    {
        public long OrderId { get; private set; }
        public decimal Amount { get; private set; }
        public string Method { get; private set; } = null!;
        public string Status { get; private set; } = PaymentStatus.Pending;
        public DateTimeOffset CreatedAt { get; private set; }

        private Payment() { } // Cho EF/AutoMapper

        // Factory method
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
                Status = PaymentStatus.Pending,
                CreatedAt = DateTimeOffset.UtcNow
            };
        }

        // Business methods
        public void MarkAsCompleted()
        {
            if (Status == PaymentStatus.Completed)
                throw new InvalidOperationException("Payment already completed.");

            Status = PaymentStatus.Completed;
        }

        public void MarkAsFailed()
        {
            if (Status == PaymentStatus.Completed)
                throw new InvalidOperationException("Cannot fail a completed payment.");

            Status = PaymentStatus.Failed;
        }
    }

    // Tạo static class để tránh "magic string"
    public static class PaymentStatus
    {
        public const string Pending = "Pending";
        public const string Completed = "Completed";
        public const string Failed = "Failed";
    }
}
