using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public partial class HolaBikeContext : IdentityDbContext<AspNetUser, IdentityRole<long>, long>
{
    public HolaBikeContext() { }

    public HolaBikeContext(DbContextOptions<HolaBikeContext> options) : base(options) { }

    // --- QUẢN LÝ CORE ---
    public DbSet<AdminProfile> AdminProfiles { get; set; }
    public DbSet<UserProfile> UserProfiles { get; set; }
    public DbSet<KycForm> KycForms { get; set; }
    public DbSet<UserDevice> UserDevices { get; set; }

    // --- NGHIỆP VỤ ĐẶT XE ---
    public DbSet<Station> Stations { get; set; }
    public DbSet<StationLog> StationLogs { get; set; }
    public DbSet<Vehicle> Vehicles { get; set; }
    public DbSet<CategoriesVehicle> CategoriesVehicles { get; set; }
    public DbSet<VehicleUsageLog> VehicleUsageLogs { get; set; }
    public DbSet<Rental> Rentals { get; set; }
    public DbSet<RentalHistory> RentalHistories { get; set; }

    // --- VÉ & THANH TOÁN ---
    public DbSet<TicketPlan> TicketPlans { get; set; }
    public DbSet<TicketPlanPrice> TicketPlanPrices { get; set; }
    public DbSet<UserTicket> UserTickets { get; set; }
    public DbSet<BookingTicket> BookingTickets { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<Wallet> Wallets { get; set; }
    public DbSet<WalletTransaction> WalletTransactions { get; set; }
    public DbSet<WalletDebt> WalletDebts { get; set; }

    // --- HỆ THỐNG & KHÁC ---
    public DbSet<News> News { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<TagNew> TagNews { get; set; }
    public DbSet<Contact> Contacts { get; set; }
    public DbSet<UserLifetimeStats> UserLifetimeStats { get; set; }

    // --- Khuyến mãi ---
    public DbSet<PromotionCampaign> PromotionCampaigns { get; set; }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // =================================================================
        // SỬA LỖI: PHÁ VỠ CHU TRÌNH XÓA TỰ ĐỘNG (CASCADE DELETE CYCLE)
        // =================================================================
        builder.Entity<BookingTicket>()
            .HasOne(bt => bt.UserTicket)
            .WithMany(ut => ut.BookingTickets)
            .HasForeignKey(bt => bt.UserTicketId)
            .OnDelete(DeleteBehavior.Restrict); 

        builder.Entity<BookingTicket>()
            .HasOne(bt => bt.Rental)
            .WithMany(r => r.BookingTickets) 
            .HasForeignKey(bt => bt.RentalId)
            .OnDelete(DeleteBehavior.Restrict);
        // =================================================================

        builder.Entity<Payment>()
            .HasIndex(p => p.GatewayTxnId, "UQ_Payment_GatewayTxnId")
            .IsUnique()
            .HasFilter("[GatewayTxnId] IS NOT NULL");

        builder.Entity<UserProfile>()
            .HasIndex(p => p.NumberCard, "UQ_UserProfile_NumberCard")
            .IsUnique()
            .HasFilter("[NumberCard] IS NOT NULL");

        OnModelCreatingPartial(builder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
