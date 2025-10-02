namespace Domain.Entities.Models
{
    public class OrderItem : BaseEntity<long>
    {
        public long OrderId { get; private set; }
        public string ProductName { get; private set; } = null!;
        public int Quantity { get; private set; }
        public decimal Price { get; private set; }

        private OrderItem() { } // constructor rỗng cho EF/ORM

        // Factory method
        public static OrderItem Create(long orderId, string productName, int quantity, decimal price)
        {
            if (string.IsNullOrWhiteSpace(productName))
                throw new ArgumentException("Product name cannot be empty.");
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero.");
            if (price < 0)
                throw new ArgumentException("Price cannot be negative.");

            return new OrderItem
            {
                OrderId = orderId,
                ProductName = productName,
                Quantity = quantity,
                Price = price
            };
        }

        // Business methods
        public void UpdateQuantity(int newQuantity)
        {
            if (newQuantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero.");
            Quantity = newQuantity;
        }

        public void UpdatePrice(decimal newPrice)
        {
            if (newPrice < 0)
                throw new ArgumentException("Price cannot be negative.");
            Price = newPrice;
        }

        public decimal GetTotalPrice()
        {
            return Quantity * Price;
        }
    }
}
