namespace Domain.Entities.Models
{
    public class CategoriesVehicle : BaseEntity<long>
    {
        public string Name { get; private set; } = null!;
        public string? Description { get; private set; }
        public bool IsActive { get; private set; }

        private readonly List<Vehicle> _vehicles = new();
        public IReadOnlyCollection<Vehicle> Vehicles => _vehicles.AsReadOnly();

        // Factory method
        public static CategoriesVehicle Create(string name, string? description)
        {
            return new CategoriesVehicle
            {
                Name = name,
                Description = description,
                IsActive = true
            };
        }

        // Business logic
        public void UpdateInfo(string name, string? description)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Tên loại xe không được để trống.");

            Name = name;
            Description = description;
        }

        public void Deactivate() => IsActive = false;
        public void Activate() => IsActive = true;

        public void AddVehicle(Vehicle vehicle)
        {
            if (vehicle == null)
                throw new ArgumentNullException(nameof(vehicle));

            _vehicles.Add(vehicle);
        }

        private CategoriesVehicle() { } // cho EF/AutoMapper
    }
}
