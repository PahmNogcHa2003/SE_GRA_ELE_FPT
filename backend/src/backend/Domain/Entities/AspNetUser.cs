using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class AspNetUser : IdentityUser<long>
    {
        [Required]
        [Precision(0)]
        public DateTimeOffset CreatedDate { get; set; }

        [InverseProperty("User")]
        public virtual ICollection<Rental> Rentals { get; set; } = new List<Rental>();

        [InverseProperty("User")]
        public virtual ICollection<Contact> Contacts { get; set; } = new List<Contact>();

        [InverseProperty("Assignee")]
        public virtual ICollection<Contact> AssignedContacts { get; set; } = new List<Contact>();

        [InverseProperty("User")]
        public virtual ICollection<KycForm> KycForms { get; set; } = new List<KycForm>();

        [InverseProperty("User")]
        public virtual ICollection<News> AuthoredNews { get; set; } = new List<News>();

        [InverseProperty("Publisher")]
        public virtual ICollection<News> PublishedNews { get; set; } = new List<News>();

        [InverseProperty("User")]
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

        [InverseProperty("User")]
        public virtual ICollection<UserDevice> UserDevices { get; set; } = new List<UserDevice>();

        [InverseProperty("User")]
        public virtual UserProfile? UserProfile { get; set; }

        [InverseProperty("User")]
        public virtual ICollection<UserSession> UserSessions { get; set; } = new List<UserSession>();

        [InverseProperty("User")]
        public virtual ICollection<UserTicket> UserTickets { get; set; } = new List<UserTicket>();

        [InverseProperty("User")]
        public virtual Wallet? Wallet { get; set; }

        [InverseProperty("User")]
        public virtual AdminProfile? AdminProfile { get; set; }

        [InverseProperty("User")]
        public virtual ICollection<WalletDebt> WalletDebts { get; set; } = new List<WalletDebt>();

        [InverseProperty("User")]
        public virtual ICollection<VehicleUsageLog> VehicleUsageLogs { get; set; } = new List<VehicleUsageLog>();
    }
}
