namespace Domain.Entities.Models
{
    public class UserAddress
    {
        public long Id { get; private set; }
        public long UserId { get; private set; }
        public string? Address { get; private set; }
        public string? City { get; private set; }
        public string? Country { get; private set; }

        private UserAddress() { } // EF/AutoMapper cần constructor rỗng

        // Factory method
        public static UserAddress Create(long userId, string? address, string? city, string? country)
        {
            if (userId <= 0)
                throw new ArgumentException("Invalid UserId", nameof(userId));

            return new UserAddress
            {
                UserId = userId,
                Address = address,
                City = city,
                Country = country
            };
        }

        // Business methods
        public void UpdateAddress(string? newAddress, string? newCity, string? newCountry)
        {
            Address = newAddress;
            City = newCity;
            Country = newCountry;
        }
    }
}
