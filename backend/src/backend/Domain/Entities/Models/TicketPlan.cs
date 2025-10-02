namespace Domain.Entities.Models
{
    public class TicketPlan : BaseEntity<long>
    {
        public string Name { get; private set; } = null!;
        public string? Description { get; private set; }
        public int DurationMinutes { get; private set; }
        public bool IsActive { get; private set; }

        private readonly List<TicketPlanPrice> _ticketPlanPrices = new();
        public IReadOnlyCollection<TicketPlanPrice> TicketPlanPrices => _ticketPlanPrices.AsReadOnly();

        private TicketPlan() { } // cho EF/AutoMapper

        // Factory method
        public static TicketPlan Create(string name, string? description, int durationMinutes)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Ticket plan name cannot be empty.", nameof(name));
            if (durationMinutes <= 0)
                throw new ArgumentException("Duration must be greater than zero.", nameof(durationMinutes));

            return new TicketPlan
            {
                Name = name,
                Description = description,
                DurationMinutes = durationMinutes,
                IsActive = true
            };
        }

        // Business methods
        public void Update(string newName, string? newDescription, int newDuration)
        {
            if (string.IsNullOrWhiteSpace(newName))
                throw new ArgumentException("Name cannot be empty.", nameof(newName));
            if (newDuration <= 0)
                throw new ArgumentException("Duration must be greater than zero.", nameof(newDuration));

            Name = newName;
            Description = newDescription;
            DurationMinutes = newDuration;
        }

        public void Deactivate() => IsActive = false;
        public void Activate() => IsActive = true;

        public void AddPrice(TicketPlanPrice price)
        {
            if (_ticketPlanPrices.Any(p => p.Id == price.Id))
                throw new InvalidOperationException("This price is already assigned to the plan.");

            _ticketPlanPrices.Add(price);
        }
    }
}
