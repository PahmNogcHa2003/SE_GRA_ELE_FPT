namespace Domain.Entities.Models
{
    public class CategoriesVehicle
    {
        public long Id { get; private set; }
        public string? Name { get; private set; }
        public string? Description { get; private set; }
        public bool IsActive { get; private set; }

        private readonly List<Vehicle> _vehicles = new();
        public IReadOnlyCollection<Vehicle> Vehicles => _vehicles.AsReadOnly();

        private CategoriesVehicle() { } // constructor rỗng cho EF/AutoMapper

    }
}
