using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace Infrastructure.Persistence;

public partial class HolaBikeContext : IdentityDbContext<AspNetUser, IdentityRole<long>, long>
{
    public HolaBikeContext()
    {
    }

    public HolaBikeContext(DbContextOptions<HolaBikeContext> options)
        : base(options)
    {
    }
    public virtual DbSet<UserVerified> UserVerifieds { get; set; }

    public virtual DbSet<AdminProfile> AdminProfiles { get; set; }

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

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=(local);Database=HolaBike;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Booking__3214EC0770A0BD14");

            entity.ToTable("Booking");

            entity.HasIndex(e => e.EndStationId, "IX_Booking_EndStationId");

            entity.HasIndex(e => e.StartStationId, "IX_Booking_StartStationId");

            entity.HasIndex(e => e.UserId, "IX_Booking_UserId");

            entity.HasIndex(e => e.VehicleId, "IX_Booking_VehicleId");

            entity.Property(e => e.BookingTime).HasPrecision(0);
            entity.Property(e => e.CreatedAt)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysdatetimeoffset())");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("Reserved");

            entity.HasOne(d => d.EndStation).WithMany(p => p.BookingEndStations)
                .HasForeignKey(d => d.EndStationId)
                .HasConstraintName("FK_Booking_EndStation");

            entity.HasOne(d => d.StartStation).WithMany(p => p.BookingStartStations)
                .HasForeignKey(d => d.StartStationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Booking_StartStation");

            entity.HasOne(d => d.User).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Booking_User");

            entity.HasOne(d => d.Vehicle).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.VehicleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Booking_Vehicle");
        });

        modelBuilder.Entity<BookingTicket>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__BookingT__3214EC071E4FB2B5");

            entity.ToTable("BookingTicket");

            entity.HasIndex(e => e.BookingId, "IX_BookingTicket_BookingId");

            entity.HasIndex(e => e.PlanPriceId, "IX_BookingTicket_PlanPriceId");

            entity.HasIndex(e => e.UserTicketId, "IX_BookingTicket_UserTicketId");

            entity.Property(e => e.AppliedAt).HasPrecision(0);
            entity.Property(e => e.VehicleType).HasMaxLength(50);

            entity.HasOne(d => d.Booking).WithMany(p => p.BookingTickets)
                .HasForeignKey(d => d.BookingId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BookingTicket_Booking");

            entity.HasOne(d => d.PlanPrice).WithMany(p => p.BookingTickets)
                .HasForeignKey(d => d.PlanPriceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BookingTicket_TPP");

            entity.HasOne(d => d.UserTicket).WithMany(p => p.BookingTickets)
                .HasForeignKey(d => d.UserTicketId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BookingTicket_UserTicket");
        });

        modelBuilder.Entity<CategoriesVehicle>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Categori__3214EC07D4E0FA58");

            entity.ToTable("CategoriesVehicle");

            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<Contact>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Contact__3214EC07DE2F6868");

            entity.ToTable("Contact");

            entity.HasIndex(e => e.UserId, "IX_Contact_UserId");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetimeoffset())");
            entity.Property(e => e.Message).HasMaxLength(500);

            entity.HasOne(d => d.User).WithMany(p => p.Contacts)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Contact_User");
        });

        modelBuilder.Entity<KycDocument>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__KycDocum__3214EC076BC33163");

            entity.ToTable("KycDocument");

            entity.HasIndex(e => e.SubmissionId, "IX_KycDocument_SubmissionId");

            entity.Property(e => e.DocPath).HasMaxLength(255);
            entity.Property(e => e.DocType).HasMaxLength(50);
            entity.Property(e => e.UploadedAt).HasDefaultValueSql("(sysdatetimeoffset())");

            entity.HasOne(d => d.Submission).WithMany(p => p.KycDocuments)
                .HasForeignKey(d => d.SubmissionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_KycDocument_Submission");
        });

        modelBuilder.Entity<KycProfile>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__KycProfi__3214EC0728C9BBF2");

            entity.ToTable("KycProfile");

            entity.HasIndex(e => e.SubmissionId, "IX_KycProfile_SubmissionId");

            entity.Property(e => e.VerifiedGender).HasMaxLength(20);
            entity.Property(e => e.VerifiedName).HasMaxLength(150);

            entity.HasOne(d => d.Submission).WithMany(p => p.KycProfiles)
                .HasForeignKey(d => d.SubmissionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_KycProfile_Submission");
        });

        modelBuilder.Entity<KycSubmission>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__KycSubmi__3214EC075C41B76A");

            entity.ToTable("KycSubmission");

            entity.HasIndex(e => e.UserId, "IX_KycSubmission_UserId");

            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Pending");
            entity.Property(e => e.SubmittedAt).HasDefaultValueSql("(sysdatetimeoffset())");

            entity.HasOne(d => d.User).WithMany(p => p.KycSubmissions)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_KycSubmission_User");
        });

        modelBuilder.Entity<News>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__News__3214EC0717F784AE");

            entity.HasIndex(e => e.AuthorId, "IX_News_AuthorId");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetimeoffset())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Title).HasMaxLength(200);

            entity.HasOne(d => d.Author).WithMany(p => p.News)
                .HasForeignKey(d => d.AuthorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_News_Author");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Order__3214EC07970C734A");

            entity.ToTable("Order");

            entity.HasIndex(e => e.UserId, "IX_Order_UserId");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetimeoffset())");
            entity.Property(e => e.OrderNo).HasMaxLength(50);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Pending");

            entity.HasOne(d => d.User).WithMany(p => p.Orders)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Order_User");
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__OrderIte__3214EC0775FA8447");

            entity.ToTable("OrderItem");

            entity.HasIndex(e => e.OrderId, "IX_OrderItem_OrderId");

            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ProductName).HasMaxLength(150);
            entity.Property(e => e.Quantity).HasDefaultValue(1);

            entity.HasOne(d => d.Order).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderItem_Order");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Payment__3214EC07EE32C633");

            entity.ToTable("Payment");

            entity.HasIndex(e => e.OrderId, "IX_Payment_OrderId");

            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetimeoffset())");
            entity.Property(e => e.Method).HasMaxLength(50);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Pending");

            entity.HasOne(d => d.Order).WithMany(p => p.Payments)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Payment_Order");
        });

        modelBuilder.Entity<Rental>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Rentals__3214EC0788FA21F4");

            entity.HasIndex(e => e.BookingId, "IX_Rentals_BookingId");

            entity.Property(e => e.Distance).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Ongoing");

            entity.HasOne(d => d.Booking).WithMany(p => p.Rentals)
                .HasForeignKey(d => d.BookingId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Rentals_Booking");
        });

        modelBuilder.Entity<Station>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Station__3214EC07C295B181");

            entity.ToTable("Station");

            entity.Property(e => e.Image).HasMaxLength(255);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Lat).HasColumnType("decimal(9, 6)");
            entity.Property(e => e.Lng).HasColumnType("decimal(9, 6)");
            entity.Property(e => e.Location).HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(150);
        });

        modelBuilder.Entity<StationLog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__StationL__3214EC07C48CFD68");

            entity.HasIndex(e => e.StationId, "IX_StationLogs_StationId");

            entity.Property(e => e.Action).HasMaxLength(50);
            entity.Property(e => e.Timestamp).HasDefaultValueSql("(sysdatetimeoffset())");

            entity.HasOne(d => d.Station).WithMany(p => p.StationLogs)
                .HasForeignKey(d => d.StationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_StationLog_Station");
        });

        modelBuilder.Entity<Tag>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Tags__3214EC07D6927A00");

            entity.Property(e => e.Name).HasMaxLength(50);

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
                        j.HasIndex(new[] { "NewsId" }, "IX_TagNew_NewsId");
                    });
        });

        modelBuilder.Entity<TicketPlan>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TicketPl__3214EC07F7F4A069");

            entity.ToTable("TicketPlan");

            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<TicketPlanPrice>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TicketPl__3214EC075961CEE6");

            entity.ToTable("TicketPlanPrice");

            entity.HasIndex(e => e.PlanId, "IX_TicketPlanPrice_PlanId");

            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.VehicleType).HasMaxLength(50);

            entity.HasOne(d => d.Plan).WithMany(p => p.TicketPlanPrices)
                .HasForeignKey(d => d.PlanId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TicketPlanPrice_Plan");
        });

        modelBuilder.Entity<UserAddress>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UserAddr__3214EC07F13D6185");

            entity.ToTable("UserAddress");

            entity.HasIndex(e => e.UserId, "IX_UserAddress_UserId");

            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.City).HasMaxLength(100);
            entity.Property(e => e.Country).HasMaxLength(100);

            entity.HasOne(d => d.User).WithMany(p => p.UserAddresses)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserAddress_User");
        });

        modelBuilder.Entity<UserDevice>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UserDevi__3214EC07D3C6E212");

            entity.ToTable("UserDevice");

            entity.HasIndex(e => e.UserId, "IX_UserDevice_UserId");

            entity.Property(e => e.DeviceId).HasMaxLength(100);
            entity.Property(e => e.DeviceType).HasMaxLength(50);

            entity.HasOne(d => d.User).WithMany(p => p.UserDevices)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserDevice_User");
        });

        modelBuilder.Entity<UserProfile>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UserProf__3214EC070F6A0589");

            entity.ToTable("UserProfile");

            entity.HasIndex(e => e.UserId, "IX_UserProfile_UserId");

            entity.Property(e => e.FullName).HasMaxLength(150);
            entity.Property(e => e.Gender).HasMaxLength(20);

            entity.HasOne(d => d.User).WithMany(p => p.UserProfiles)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserProfile_User");
        });

        modelBuilder.Entity<UserSession>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UserSess__3214EC07FD5AF48C");

            entity.ToTable("UserSession");

            entity.HasIndex(e => e.UserId, "IX_UserSession_UserId");

            entity.Property(e => e.SessionToken).HasMaxLength(200);

            entity.HasOne(d => d.User).WithMany(p => p.UserSessions)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserSession_User");
        });

        modelBuilder.Entity<UserTicket>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UserTick__3214EC07A55923E8");

            entity.ToTable("UserTicket");

            entity.HasIndex(e => e.PlanPriceId, "IX_UserTicket_PlanPriceId");

            entity.HasIndex(e => e.UserId, "IX_UserTicket_UserId");

            entity.HasOne(d => d.PlanPrice).WithMany(p => p.UserTickets)
                .HasForeignKey(d => d.PlanPriceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserTicket_Price");

            entity.HasOne(d => d.User).WithMany(p => p.UserTickets)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserTicket_User");
        });

        modelBuilder.Entity<Vehicle>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Vehicle__3214EC0734EACA34");

            entity.ToTable("Vehicle");

            entity.HasIndex(e => e.BikeCode, "IX_Vehicle_BikeCode").IsUnique();

            entity.HasIndex(e => e.CategoryId, "IX_Vehicle_CategoryId");

            entity.HasIndex(e => e.StationId, "IX_Vehicle_StationId");

            entity.Property(e => e.BikeCode).HasMaxLength(50);
            entity.Property(e => e.CreatedAt)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysdatetimeoffset())");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("Available");

            entity.HasOne(d => d.Category).WithMany(p => p.Vehicles)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("FK_Vehicle_Category");

            entity.HasOne(d => d.Station).WithMany(p => p.Vehicles)
                .HasForeignKey(d => d.StationId)
                .HasConstraintName("FK_Vehicle_Station");
        });

        modelBuilder.Entity<VehicleUsageLog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__VehicleU__3214EC07E37D2CE1");

            entity.HasIndex(e => e.BookingId, "IX_VehicleUsageLogs_BookingId");

            entity.HasIndex(e => e.VehicleId, "IX_VehicleUsageLogs_VehicleId");

            entity.Property(e => e.Status).HasMaxLength(20);
            entity.Property(e => e.Timestamp).HasDefaultValueSql("(sysdatetimeoffset())");

            entity.HasOne(d => d.Booking).WithMany(p => p.VehicleUsageLogs)
                .HasForeignKey(d => d.BookingId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UsageLog_Booking");

            entity.HasOne(d => d.Vehicle).WithMany(p => p.VehicleUsageLogs)
                .HasForeignKey(d => d.VehicleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UsageLog_Vehicle");
        });

        modelBuilder.Entity<Wallet>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Wallet__3214EC0718574291");

            entity.ToTable("Wallet");

            entity.HasIndex(e => e.UserId, "IX_Wallet_UserId");

            entity.Property(e => e.Balance).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(sysdatetimeoffset())");

            entity.HasOne(d => d.User).WithMany(p => p.Wallets)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Wallet_User");
        });

        modelBuilder.Entity<WalletTransaction>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__WalletTr__3214EC0730F16E79");

            entity.ToTable("WalletTransaction");

            entity.HasIndex(e => e.WalletId, "IX_WalletTransaction_WalletId");

            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetimeoffset())");
            entity.Property(e => e.Type).HasMaxLength(50);

            entity.HasOne(d => d.Wallet).WithMany(p => p.WalletTransactions)
                .HasForeignKey(d => d.WalletId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_WalletTransaction_Wallet");
        });
        modelBuilder.Entity<AdminProfile>(entity =>
        {
            entity.ToTable("AdminProfile");

            entity.HasKey(e => e.UserId).HasName("PK_AdminProfile");

            entity.Property(e => e.FullName).HasMaxLength(150);
            entity.Property(e => e.Position).HasMaxLength(100);
            entity.Property(e => e.AvatarUrl).HasMaxLength(255);

            entity.Property(e => e.CreatedAt)
                  .HasPrecision(0)
                  .HasDefaultValueSql("(sysdatetimeoffset())");
            entity.Property(e => e.UpdatedAt)
                  .HasPrecision(0)
                  .HasDefaultValueSql("(sysdatetimeoffset())");

            entity.HasOne(d => d.User)
                  .WithOne(p => p.AdminProfile)
                  .HasForeignKey<AdminProfile>(d => d.UserId)
                  .OnDelete(DeleteBehavior.Cascade) 
                  .HasConstraintName("FK_AdminProfile_User");
        });

        modelBuilder.Entity<UserVerified>(entity =>
        {
            entity.ToTable("UserVerified");
            entity.HasKey(e => e.Id).HasName("PK_UserVerified");

     
            entity.Property(e => e.Type).HasMaxLength(50);
            entity.Property(e => e.Number).HasMaxLength(100);
            entity.Property(e => e.FullName).HasMaxLength(150);
            entity.Property(e => e.PlaceOfBirth).HasMaxLength(150);
            entity.Property(e => e.IssuedBy).HasMaxLength(100);
            entity.Property(e => e.VerificationMethod).HasMaxLength(50);

            entity.Property(e => e.CreatedAt)
                  .HasDefaultValueSql("(sysdatetimeoffset())");
            entity.Property(e => e.UpdatedAt)
                  .HasDefaultValueSql("(sysdatetimeoffset())");


            entity.HasIndex(e => e.UserId)
                  .IsUnique()
                  .HasDatabaseName("UX_UserVerified_UserId");                

            entity.HasIndex(e => e.VerifiedBy)
                  .HasDatabaseName("IX_UserVerified_VerifiedBy");

            entity.HasIndex(e => e.SubmissionId)
                  .HasDatabaseName("IX_UserVerified_SubmissionId");

     
            entity.HasIndex(e => new { e.VerifiedAt, e.Type })
                  .HasDatabaseName("IX_UserVerified_VerifiedAt_Type");


            entity.HasOne(d => d.User)
                  .WithMany(p => p.UserVerifieds)
                  .HasForeignKey(d => d.UserId)
                  .OnDelete(DeleteBehavior.Cascade)
                  .HasConstraintName("FK_UserVerified_User");

     
            entity.HasOne(d => d.VerifiedByUser)
                  .WithMany(p => p.VerifiedUsers)          
                  .HasForeignKey(d => d.VerifiedBy)
                  .OnDelete(DeleteBehavior.NoAction)    
                  .HasConstraintName("FK_UserVerified_VerifiedBy");

    
            entity.HasOne(d => d.Submission)
                  .WithMany()                               
                  .HasForeignKey(d => d.SubmissionId)
                  .OnDelete(DeleteBehavior.SetNull)
                  .HasConstraintName("FK_UserVerified_Submission");
        });



        OnModelCreatingPartial(modelBuilder);

    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
