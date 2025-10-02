using System;
using System.Collections.Generic;

namespace Domain.Entities.Models
{
    public class AspNetUser : BaseEntity<long>
    {
        public string? UserName { get; set; }
        public string? NormalizedUserName { get; set; }
        public string? Email { get; set; }
        public string? NormalizedEmail { get; set; }
        public bool EmailConfirmed { get; set; }
        public string? PasswordHash { get; set; }
        public string? SecurityStamp { get; set; }
        public string? ConcurrencyStamp { get; set; }
        public string? PhoneNumber { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public DateTimeOffset? LockoutEnd { get; set; }
        public bool LockoutEnabled { get; set; }
        public int AccessFailedCount { get; set; }

        // Navigation properties (Aggregate relationships)
        public ICollection<Booking> Bookings { get; private set; } = new List<Booking>();
        public ICollection<Contact> Contacts { get; private set; } = new List<Contact>();
        public ICollection<KycSubmission> KycSubmissions { get; private set; } = new List<KycSubmission>();
        public ICollection<News> News { get; private set; } = new List<News>();
        public ICollection<Order> Orders { get; private set; } = new List<Order>();
        public ICollection<UserAddress> UserAddresses { get; private set; } = new List<UserAddress>();
        public ICollection<UserDevice> UserDevices { get; private set; } = new List<UserDevice>();
        public ICollection<UserProfile> UserProfiles { get; private set; } = new List<UserProfile>();
        public ICollection<UserSession> UserSessions { get; private set; } = new List<UserSession>();
        public ICollection<UserTicket> UserTickets { get; private set; } = new List<UserTicket>();
        public ICollection<Wallet> Wallets { get; private set; } = new List<Wallet>();

        // ✅ Factory method để tạo user mới (Domain-driven design style)
        public static AspNetUser Create(string userName, string email)
        {
            return new AspNetUser
            {
                UserName = userName,
                NormalizedUserName = userName.ToUpperInvariant(),
                Email = email,
                NormalizedEmail = email.ToUpperInvariant(),
                SecurityStamp = Guid.NewGuid().ToString(),
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                EmailConfirmed = false,
                PhoneNumberConfirmed = false,
                TwoFactorEnabled = false,
                LockoutEnabled = true,
                AccessFailedCount = 0
            };
        }

        // private constructor để enforce dùng Factory
        private AspNetUser() { }
    }
}
