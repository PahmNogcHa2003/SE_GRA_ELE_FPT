using System;
using System.Collections.Generic;
using Infrastructure.Persistence.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public partial class AppDbContext : IdentityDbContext<AspNetUser, IdentityRole<long>, long>
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AspNetUser> AspNetUsers { get; set; }

    public virtual DbSet<Booking> Bookings { get; set; }

    public virtual DbSet<BookingTicket> BookingTickets { get; set; }

    public virtual DbSet<CategoriesVehicle> CategoriesVehicles { get; set; }

    public virtual DbSet<Contact> Contacts { get; set; }

    public virtual DbSet<KycDocument> KycDocuments { get; set; }

    public virtual DbSet<KycProfile> KycProfiles { get; set; }

    public virtual DbSet<KycSubmission> KycSubmissions { get; set; }

    public virtual DbSet<News> News { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderItem> OrderItems { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Rental> Rentals { get; set; }

    public virtual DbSet<Station> Stations { get; set; }

    public virtual DbSet<StationLog> StationLogs { get; set; }

    public virtual DbSet<Tag> Tags { get; set; }

    public virtual DbSet<TicketPlan> TicketPlans { get; set; }

    public virtual DbSet<TicketPlanPrice> TicketPlanPrices { get; set; }

    public virtual DbSet<UserAddress> UserAddresses { get; set; }

    public virtual DbSet<UserDevice> UserDevices { get; set; }

    public virtual DbSet<UserProfile> UserProfiles { get; set; }

    public virtual DbSet<UserSession> UserSessions { get; set; }

    public virtual DbSet<UserTicket> UserTickets { get; set; }

    public virtual DbSet<Vehicle> Vehicles { get; set; }

    public virtual DbSet<VehicleUsageLog> VehicleUsageLogs { get; set; }

    public virtual DbSet<Wallet> Wallets { get; set; }

    public virtual DbSet<WalletTransaction> WalletTransactions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<AspNetUser>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__AspNetUs__3214EC072EDEC32C");

            entity.Property(e => e.LockoutEnabled).HasDefaultValue(true);
        });

        modelBuilder.Entity<IdentityUserLogin<long>>(b =>
        {
            b.HasKey(l => new { l.LoginProvider, l.ProviderKey });
        });

        modelBuilder.Entity<IdentityUserRole<long>>(b =>
        {
            b.HasKey(r => new { r.UserId, r.RoleId });
        });

        modelBuilder.Entity<IdentityUserToken<long>>(b =>
        {
            b.HasKey(t => new { t.UserId, t.LoginProvider, t.Name });
        });

        modelBuilder.Entity<IdentityRoleClaim<long>>(b =>
        {
            b.HasKey(rc => rc.Id);
        });

        modelBuilder.Entity<IdentityUserClaim<long>>(b =>
        {
            b.HasKey(uc => uc.Id);
        });

        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Booking__3214EC0770A0BD14");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetimeoffset())");
            entity.Property(e => e.Status).HasDefaultValue("Reserved");

            entity.HasOne(d => d.EndStation).WithMany(p => p.BookingEndStations).HasConstraintName("FK_Booking_EndStation");

            entity.HasOne(d => d.StartStation).WithMany(p => p.BookingStartStations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Booking_StartStation");

            entity.HasOne(d => d.User).WithMany(p => p.Bookings)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Booking_User");

            entity.HasOne(d => d.Vehicle).WithMany(p => p.Bookings)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Booking_Vehicle");
        });

        modelBuilder.Entity<BookingTicket>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__BookingT__3214EC071E4FB2B5");

            entity.HasOne(d => d.Booking).WithMany(p => p.BookingTickets)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BookingTicket_Booking");

            entity.HasOne(d => d.PlanPrice).WithMany(p => p.BookingTickets)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BookingTicket_TPP");

            entity.HasOne(d => d.UserTicket).WithMany(p => p.BookingTickets)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BookingTicket_UserTicket");
        });

        modelBuilder.Entity<CategoriesVehicle>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Categori__3214EC07D4E0FA58");

            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });

        modelBuilder.Entity<Contact>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Contact__3214EC07DE2F6868");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetimeoffset())");

            entity.HasOne(d => d.User).WithMany(p => p.Contacts)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Contact_User");
        });

        modelBuilder.Entity<KycDocument>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__KycDocum__3214EC076BC33163");

            entity.Property(e => e.UploadedAt).HasDefaultValueSql("(sysdatetimeoffset())");

            entity.HasOne(d => d.Submission).WithMany(p => p.KycDocuments)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_KycDocument_Submission");
        });

        modelBuilder.Entity<KycProfile>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__KycProfi__3214EC0728C9BBF2");

            entity.HasOne(d => d.Submission).WithMany(p => p.KycProfiles)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_KycProfile_Submission");
        });

        modelBuilder.Entity<KycSubmission>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__KycSubmi__3214EC075C41B76A");

            entity.Property(e => e.Status).HasDefaultValue("Pending");
            entity.Property(e => e.SubmittedAt).HasDefaultValueSql("(sysdatetimeoffset())");

            entity.HasOne(d => d.User).WithMany(p => p.KycSubmissions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_KycSubmission_User");
        });

        modelBuilder.Entity<News>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__News__3214EC0717F784AE");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetimeoffset())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);

            entity.HasOne(d => d.Author).WithMany(p => p.News)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_News_Author");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Order__3214EC07970C734A");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetimeoffset())");
            entity.Property(e => e.Status).HasDefaultValue("Pending");

            entity.HasOne(d => d.User).WithMany(p => p.Orders)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Order_User");
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__OrderIte__3214EC0775FA8447");

            entity.Property(e => e.Quantity).HasDefaultValue(1);

            entity.HasOne(d => d.Order).WithMany(p => p.OrderItems)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderItem_Order");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Payment__3214EC07EE32C633");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetimeoffset())");
            entity.Property(e => e.Status).HasDefaultValue("Pending");

            entity.HasOne(d => d.Order).WithMany(p => p.Payments)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Payment_Order");
        });

        modelBuilder.Entity<Rental>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Rentals__3214EC0788FA21F4");

            entity.Property(e => e.Status).HasDefaultValue("Ongoing");

            entity.HasOne(d => d.Booking).WithMany(p => p.Rentals)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Rentals_Booking");
        });

        modelBuilder.Entity<Station>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Station__3214EC07C295B181");

            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });

        modelBuilder.Entity<StationLog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__StationL__3214EC07C48CFD68");

            entity.Property(e => e.Timestamp).HasDefaultValueSql("(sysdatetimeoffset())");

            entity.HasOne(d => d.Station).WithMany(p => p.StationLogs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_StationLog_Station");
        });

        modelBuilder.Entity<Tag>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Tags__3214EC07D6927A00");

            entity.HasMany(d => d.News).WithMany(p => p.Tags)
                .UsingEntity<Dictionary<string, object>>(
                    "TagNew",
                    r => r.HasOne<News>().WithMany()
                        .HasForeignKey("NewsId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_TagNew_News"),
                    l => l.HasOne<Tag>().WithMany()
                        .HasForeignKey("TagId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_TagNew_Tag"),
                    j =>
                    {
                        j.HasKey("TagId", "NewsId").HasName("PK__TagNew__4C28127383B3AF5F");
                        j.ToTable("TagNew");
                    });
        });

        modelBuilder.Entity<TicketPlan>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TicketPl__3214EC07F7F4A069");

            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });

        modelBuilder.Entity<TicketPlanPrice>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TicketPl__3214EC075961CEE6");

            entity.HasOne(d => d.Plan).WithMany(p => p.TicketPlanPrices)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TicketPlanPrice_Plan");
        });

        modelBuilder.Entity<UserAddress>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UserAddr__3214EC07F13D6185");

            entity.HasOne(d => d.User).WithMany(p => p.UserAddresses)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserAddress_User");
        });

        modelBuilder.Entity<UserDevice>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UserDevi__3214EC07D3C6E212");

            entity.HasOne(d => d.User).WithMany(p => p.UserDevices)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserDevice_User");
        });

        modelBuilder.Entity<UserProfile>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UserProf__3214EC070F6A0589");

            entity.HasOne(d => d.User).WithMany(p => p.UserProfiles)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserProfile_User");
        });

        modelBuilder.Entity<UserSession>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UserSess__3214EC07FD5AF48C");

            entity.HasOne(d => d.User).WithMany(p => p.UserSessions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserSession_User");
        });

        modelBuilder.Entity<UserTicket>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UserTick__3214EC07A55923E8");

            entity.HasOne(d => d.PlanPrice).WithMany(p => p.UserTickets)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserTicket_Price");

            entity.HasOne(d => d.User).WithMany(p => p.UserTickets)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserTicket_User");
        });

        modelBuilder.Entity<Vehicle>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Vehicle__3214EC0734EACA34");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetimeoffset())");
            entity.Property(e => e.Status).HasDefaultValue("Available");

            entity.HasOne(d => d.Category).WithMany(p => p.Vehicles).HasConstraintName("FK_Vehicle_Category");

            entity.HasOne(d => d.Station).WithMany(p => p.Vehicles).HasConstraintName("FK_Vehicle_Station");
        });

        modelBuilder.Entity<VehicleUsageLog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__VehicleU__3214EC07E37D2CE1");

            entity.Property(e => e.Timestamp).HasDefaultValueSql("(sysdatetimeoffset())");

            entity.HasOne(d => d.Booking).WithMany(p => p.VehicleUsageLogs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UsageLog_Booking");

            entity.HasOne(d => d.Vehicle).WithMany(p => p.VehicleUsageLogs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UsageLog_Vehicle");
        });

        modelBuilder.Entity<Wallet>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Wallet__3214EC0718574291");

            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(sysdatetimeoffset())");

            entity.HasOne(d => d.User).WithMany(p => p.Wallets)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Wallet_User");
        });

        modelBuilder.Entity<WalletTransaction>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__WalletTr__3214EC0730F16E79");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetimeoffset())");

            entity.HasOne(d => d.Wallet).WithMany(p => p.WalletTransactions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_WalletTransaction_Wallet");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
