namespace Domain.Entities.Models
{
    public class TicketPlanPrice : BaseEntity<long>
    {
        public long PlanId { get; private set; }
        public string VehicleType { get; private set; } = null!;
        public decimal Price { get; private set; }

        private readonly List<BookingTicket> _bookingTickets = new();
        public IReadOnlyCollection<BookingTicket> BookingTickets => _bookingTickets.AsReadOnly();

        private readonly List<UserTicket> _userTickets = new();
        public IReadOnlyCollection<UserTicket> UserTickets => _userTickets.AsReadOnly();

        private TicketPlanPrice() { } // EF/AutoMapper cần

        // Factory
        public static TicketPlanPrice Create(long planId, string vehicleType, decimal price)
        {
            if (string.IsNullOrWhiteSpace(vehicleType))
                throw new ArgumentException("Vehicle type cannot be empty.", nameof(vehicleType));
            if (price <= 0)
                throw new ArgumentException("Price must be greater than zero.", nameof(price));

            return new TicketPlanPrice
            {
                PlanId = planId,
                VehicleType = vehicleType,
                Price = price
            };
        }

        // Business methods
        public void UpdatePrice(decimal newPrice)
        {
            if (newPrice <= 0)
                throw new ArgumentException("Price must be greater than zero.", nameof(newPrice));

            Price = newPrice;
        }

        public void ChangeVehicleType(string newVehicleType)
        {
            if (string.IsNullOrWhiteSpace(newVehicleType))
                throw new ArgumentException("Vehicle type cannot be empty.", nameof(newVehicleType));

            VehicleType = newVehicleType;
        }

        public void AddBookingTicket(BookingTicket bookingTicket)
        {
            if (_bookingTickets.Any(bt => bt.Id == bookingTicket.Id))
                return;

            _bookingTickets.Add(bookingTicket);
        }

        public void AddUserTicket(UserTicket userTicket)
        {
            if (_userTickets.Any(ut => ut.Id == userTicket.Id))
                return;

            _userTickets.Add(userTicket);
        }
    }
}
