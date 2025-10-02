namespace Domain.Entities.Models
{
    public class Station : BaseEntity<long>
    {
        public string Name { get; private set; } = null!;
        public string? Location { get; private set; }
        public int Capacity { get; private set; }
        public decimal Lat { get; private set; }
        public decimal Lng { get; private set; }
        public bool IsActive { get; private set; }
        public string? Image { get; private set; }

        private readonly List<Booking> _startBookings = new();
        private readonly List<Booking> _endBookings = new();
        private readonly List<StationLog> _stationLogs = new();
        private readonly List<Vehicle> _vehicles = new();

        public IReadOnlyCollection<Booking> StartBookings => _startBookings.AsReadOnly();
        public IReadOnlyCollection<Booking> EndBookings => _endBookings.AsReadOnly();
        public IReadOnlyCollection<StationLog> StationLogs => _stationLogs.AsReadOnly();
        public IReadOnlyCollection<Vehicle> Vehicles => _vehicles.AsReadOnly();

        private Station() { } // cho EF/AutoMapper

        // ✅ Factory method
        public static Station Create(string name, string? location, int capacity, decimal lat, decimal lng, string? image = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Station name cannot be empty.");
            if (capacity < 0)
                throw new ArgumentException("Capacity must be non-negative.");

            return new Station
            {
                Name = name,
                Location = location,
                Capacity = capacity,
                Lat = lat,
                Lng = lng,
                IsActive = true,
                Image = image
            };
        }

        // ✅ Business methods
        public void UpdateInfo(string name, string? location, int capacity, decimal lat, decimal lng, string? image = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Station name cannot be empty.");
            if (capacity < 0)
                throw new ArgumentException("Capacity must be non-negative.");

            Name = name;
            Location = location;
            Capacity = capacity;
            Lat = lat;
            Lng = lng;
            Image = image;
        }

        public void Deactivate() => IsActive = false;

        public void Activate() => IsActive = true;

        public void AddVehicle(Vehicle vehicle)
        {
            if (_vehicles.Count >= Capacity)
                throw new InvalidOperationException("Station is at full capacity.");
            if (!_vehicles.Contains(vehicle))
                _vehicles.Add(vehicle);
        }

        public void RemoveVehicle(Vehicle vehicle)
        {
            if (!_vehicles.Contains(vehicle))
                throw new InvalidOperationException("Vehicle not found in this station.");

            _vehicles.Remove(vehicle);
        }

        public void AddStationLog(StationLog log)
        {
            _stationLogs.Add(log);
        }
    }
}
