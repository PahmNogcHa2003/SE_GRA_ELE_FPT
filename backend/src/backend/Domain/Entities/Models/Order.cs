namespace Domain.Entities.Models
{
    public class Order : BaseEntity<long>
    {
        public long UserId { get; private set; }
        public string OrderNo { get; private set; } = null!;
        public string Status { get; private set; } = OrderStatus.Pending;
        public DateTimeOffset CreatedAt { get; private set; }
        public DateTimeOffset? UpdatedAt { get; private set; }

        private readonly List<OrderItem> _orderItems = new();
        public IReadOnlyCollection<OrderItem> OrderItems => _orderItems.AsReadOnly();

        private readonly List<Payment> _payments = new();
        public IReadOnlyCollection<Payment> Payments => _payments.AsReadOnly();

        private Order() { } // constructor rỗng cho EF/AutoMapper

        // Factory method: tạo order mới
        public static Order Create(long userId, string orderNo)
        {
            if (string.IsNullOrWhiteSpace(orderNo))
                throw new ArgumentException("Order number cannot be empty.");

            return new Order
            {
                UserId = userId,
                OrderNo = orderNo,
                Status = OrderStatus.Pending,
                CreatedAt = DateTimeOffset.UtcNow
            };
        }

        // Business methods
        public void AddItem(OrderItem item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            _orderItems.Add(item);
            Touch();
        }

        public void RemoveItem(OrderItem item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            _orderItems.Remove(item);
            Touch();
        }

        public void AddPayment(Payment payment)
        {
            if (payment == null) throw new ArgumentNullException(nameof(payment));
            _payments.Add(payment);
            Touch();
        }

        public void UpdateStatus(string newStatus)
        {
            if (string.IsNullOrWhiteSpace(newStatus))
                throw new ArgumentException("Status cannot be empty.");

            Status = newStatus;
            Touch();
        }

        private void Touch()
        {
            UpdatedAt = DateTimeOffset.UtcNow;
        }
    }

    // Dùng static class để tránh magic string
    public static class OrderStatus
    {
        public const string Pending = "Pending";
        public const string Paid = "Paid";
        public const string Shipped = "Shipped";
        public const string Completed = "Completed";
        public const string Cancelled = "Cancelled";
    }
}
