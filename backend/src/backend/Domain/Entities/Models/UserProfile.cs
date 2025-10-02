namespace Domain.Entities.Models
{
    public class UserProfile : BaseEntity<long>
    {
        public long UserId { get; private set; }
        public string? FullName { get; private set; }
        public DateOnly? Dob { get; private set; }
        public string? Gender { get; private set; }

        private UserProfile() { } // EF/AutoMapper cần

        // Factory method
        public static UserProfile Create(long userId, string? fullName, DateOnly? dob, string? gender)
        {
            if (userId <= 0)
                throw new ArgumentException("Invalid UserId", nameof(userId));

            return new UserProfile
            {
                UserId = userId,
                FullName = fullName,
                Dob = dob,
                Gender = gender
            };
        }

        // Business methods
        public void UpdateProfile(string? fullName, DateOnly? dob, string? gender)
        {
            FullName = fullName;
            Dob = dob;
            Gender = gender;
        }
    }
}
