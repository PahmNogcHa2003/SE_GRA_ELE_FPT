using System;
using System.Collections.Generic;

namespace Domain.Entities.Models
{
    public class AspNetUser
    {
        public long Id { get; set; }
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

        // Navigation properties
        public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
        public virtual ICollection<Contact> Contacts { get; set; } = new List<Contact>();
        public virtual ICollection<KycSubmission> KycSubmissions { get; set; } = new List<KycSubmission>();
        public virtual ICollection<News> News { get; set; } = new List<News>();
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
        public virtual ICollection<UserAddress> UserAddresses { get; set; } = new List<UserAddress>();
        public virtual ICollection<UserDevice> UserDevices { get; set; } = new List<UserDevice>();
        public virtual ICollection<UserProfile> UserProfiles { get; set; } = new List<UserProfile>();
        public virtual ICollection<UserSession> UserSessions { get; set; } = new List<UserSession>();
        public virtual ICollection<UserTicket> UserTickets { get; set; } = new List<UserTicket>();
        public virtual ICollection<Wallet> Wallets { get; set; } = new List<Wallet>();

        private AspNetUser() { } // private constructor nếu muốn enforce factory hoặc ORM
    }
}
