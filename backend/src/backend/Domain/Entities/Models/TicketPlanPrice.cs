
namespace Domain.Entities.Models
{
    public class TicketPlanPrice
    {
        public long Id { get; private set; }
        public long PlanId { get; private set; }
        public string? VehicleType { get; private set; }
        public decimal Price { get; private set; }

        private readonly List<BookingTicket> _bookingTickets = new();
        public IReadOnlyCollection<BookingTicket> BookingTickets => _bookingTickets.AsReadOnly();

        private readonly List<UserTicket> _userTickets = new();
        public IReadOnlyCollection<UserTicket> UserTickets => _userTickets.AsReadOnly();

        private TicketPlanPrice() { } // EF/AutoMapper cần

        // Factory
        public static TicketPlanPrice Create(long planId, string? vehicleType, decimal price)
        {
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

        public void ChangeVehicleType(string? newVehicleType)
        {
            VehicleType = newVehicleType;
        }

        public void AddBookingTicket(BookingTicket bookingTicket)
        {
            if (!_bookingTickets.Contains(bookingTicket))
                _bookingTickets.Add(bookingTicket);
        }

        public void AddUserTicket(UserTicket userTicket)
        {
            if (!_userTickets.Contains(userTicket))
                _userTickets.Add(userTicket);
        }
    }
}
