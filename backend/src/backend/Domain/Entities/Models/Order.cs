
namespace Domain.Entities.Models
{
    public class Order
    {
        public long Id { get; private set; }
        public long UserId { get; private set; }
        public string OrderNo { get; private set; } = null!;
        public string Status { get; private set; } = "Pending";
        public DateTimeOffset CreatedAt { get; private set; }

        private readonly List<OrderItem> _orderItems = new();
        public IReadOnlyCollection<OrderItem> OrderItems => _orderItems.AsReadOnly();

        private readonly List<Payment> _payments = new();
        public IReadOnlyCollection<Payment> Payments => _payments.AsReadOnly();

        private Order() { } // constructor rỗng cho EF/AutoMapper

        // Factory method: tạo order mới
        public static Order Create(long userId, string orderNo)
        {
            return new Order
            {
                UserId = userId,
                OrderNo = orderNo,
                Status = "Pending",
                CreatedAt = DateTimeOffset.UtcNow
            };
        }

        // Business methods
        public void AddItem(OrderItem item)
        {
            _orderItems.Add(item);
        }

        public void AddPayment(Payment payment)
        {
            _payments.Add(payment);
        }

        public void UpdateStatus(string newStatus)
        {
            if (string.IsNullOrWhiteSpace(newStatus))
                throw new ArgumentException("Status cannot be empty.");

            Status = newStatus;
        }
    }
}
