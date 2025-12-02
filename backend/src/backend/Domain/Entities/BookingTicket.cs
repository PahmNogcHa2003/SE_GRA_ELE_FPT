using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Domain.Entities;

[Table("BookingTicket")]
public class BookingTicket : BaseEntity<long>
{
    [Required]
    public long RentalId { get; set; }

    [Required]
    public long UserTicketId { get; set; }

    [Required]
    [Column(TypeName = "decimal(18, 2)")]
    public decimal PlanPrice { get; set; }

    [StringLength(50)]
    public string? VehicleType { get; set; }

    public int? UsedMinutes { get; set; }

    [Column(TypeName = "int")]
    public int? OverusedMinutes { get; set; } 

    [Column(TypeName = "decimal(18,2)")]
    public decimal? OverusedFee { get; set; } 

    [Precision(0)]
    public DateTimeOffset? AppliedAt { get; set; }

    [ForeignKey(nameof(RentalId))]
    [InverseProperty(nameof(Rental.BookingTickets))]
    public Rental Rental { get; set; } = null!;

    [ForeignKey(nameof(UserTicketId))]
    [InverseProperty(nameof(UserTicket.BookingTickets))]
    public UserTicket UserTicket { get; set; } = null!;

    // Constructor để tạo bản ghi khi áp dụng vé (ĐÚNG)
    public BookingTicket(long rentalId, long userTicketId, decimal planPrice, string vehicleType, DateTimeOffset appliedAt)
    {
        if (rentalId <= 0 || userTicketId <= 0)
        {
            throw new ArgumentException("RentalId and UserTicketId must be positive.");
        }

        RentalId = rentalId;
        UserTicketId = userTicketId;
        PlanPrice = planPrice;
        VehicleType = vehicleType;
        AppliedAt = appliedAt;
        UsedMinutes = 0; // Khởi tạo bằng 0 phút
    }

    // Thêm constructor không tham số để tương thích với EF Core hoặc Factory (Tùy chọn)
    public BookingTicket() { }

    // Phương thức logic nghiệp vụ: Cập nhật thời gian đã sử dụng (ĐÚNG)
    public void UpdateUsedMinutes(int minutes)
    {
        if (minutes < 0)
        {
            throw new ArgumentException("UsedMinutes cannot be negative.", nameof(minutes));
        }
        UsedMinutes = minutes;
    }
}