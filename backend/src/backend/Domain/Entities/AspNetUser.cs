using Microsoft.AspNetCore.Identity;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class AspNetUser : IdentityUser<long>
    {
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedDate { get; set; }

        [InverseProperty("User")]
        public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

        [InverseProperty("User")]
        public virtual ICollection<Contact> Contacts { get; set; } = new List<Contact>();

        [InverseProperty("User")]
        public virtual ICollection<KycSubmission> KycSubmissions { get; set; } = new List<KycSubmission>();

        [InverseProperty("Author")]
        public virtual ICollection<News> News { get; set; } = new List<News>();

        [InverseProperty("User")]
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

        [InverseProperty("User")]
        public virtual ICollection<UserAddress> UserAddresses { get; set; } = new List<UserAddress>();

        [InverseProperty("User")]
        public virtual ICollection<UserDevice> UserDevices { get; set; } = new List<UserDevice>();

        [InverseProperty("User")]
        public virtual ICollection<UserProfile> UserProfiles { get; set; } = new List<UserProfile>();

        [InverseProperty("User")]
        public virtual ICollection<UserSession> UserSessions { get; set; } = new List<UserSession>();

        [InverseProperty("User")]
        public virtual ICollection<UserTicket> UserTickets { get; set; } = new List<UserTicket>();

        [InverseProperty("User")]
        public virtual ICollection<Wallet> Wallets { get; set; } = new List<Wallet>();

        [InverseProperty("User")]
        public virtual AdminProfile? AdminProfile { get; set; }

        [InverseProperty("User")]
        public virtual ICollection<UserVerified> UserVerifieds { get; set; } = new List<UserVerified>();

        [InverseProperty("VerifiedByUser")]
        public virtual ICollection<UserVerified> VerifiedUsers { get; set; } = new List<UserVerified>();
    }
}
