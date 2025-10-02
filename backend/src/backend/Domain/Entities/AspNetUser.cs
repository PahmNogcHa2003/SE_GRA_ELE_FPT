using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Domain.Entities;

[Index("NormalizedEmail", Name = "EmailIndex")]
public partial class AspNetUser : BaseEntity<long>
{
    [StringLength(256)]
    public string? UserName { get; set; }

    [StringLength(256)]
    public string? NormalizedUserName { get; set; }

    [StringLength(256)]
    public string? Email { get; set; }

    [StringLength(256)]
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

    [InverseProperty("User")]
    public virtual ICollection<AspNetUserClaim> AspNetUserClaims { get; set; } = new List<AspNetUserClaim>();

    [InverseProperty("User")]
    public virtual ICollection<AspNetUserLogin> AspNetUserLogins { get; set; } = new List<AspNetUserLogin>();

    [InverseProperty("User")]
    public virtual ICollection<AspNetUserToken> AspNetUserTokens { get; set; } = new List<AspNetUserToken>();

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

    [ForeignKey("UserId")]
    [InverseProperty("Users")]
    public virtual ICollection<AspNetRole> Roles { get; set; } = new List<AspNetRole>();
}
