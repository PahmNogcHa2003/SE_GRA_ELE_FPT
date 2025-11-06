using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Domain.Entities;

[Table("UserTicket")]
public partial class UserTicket : BaseEntity<long>
{
    [Required]
    public long UserId { get; set; }

    [Required]
    public long PlanPriceId { get; set; }

    [StringLength(50)]
    public string? SerialCode { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? PurchasedPrice { get; set; }

    [Required, StringLength(20), Unicode(false)]
    public string Status { get; set; } = "Ready"; // Ready | Active | Used | Expired | Refunded

    [Precision(0)]
    public DateTimeOffset? ActivatedAt { get; set; }

    // ✅ NEW (Day/Month dùng để hiển thị & kiểm tra hiệu lực)
    [Precision(0)]
    public DateTimeOffset? ValidFrom { get; set; }

    [Precision(0)]
    public DateTimeOffset? ValidTo { get; set; }

    // (tùy chọn nếu bạn dùng) hết hạn “vé” nói chung
    [Precision(0)]
    public DateTimeOffset? ExpiresAt { get; set; }

    // ✅ NEW (vé lượt)
    [Precision(0)]
    public DateTimeOffset? ActivationDeadline { get; set; }

    public int? RemainingMinutes { get; set; }
    public int? RemainingRides { get; set; }

    [Required, Precision(0)]
    public DateTimeOffset CreatedAt { get; set; }

    [Timestamp]
    public byte[]? RowVer { get; set; }

    [InverseProperty(nameof(BookingTicket.UserTicket))]
    public virtual ICollection<BookingTicket> BookingTickets { get; set; } = new List<BookingTicket>();

    [ForeignKey(nameof(PlanPriceId))]
    [InverseProperty(nameof(TicketPlanPrice.UserTickets))]
    public virtual TicketPlanPrice PlanPrice { get; set; } = null!;

    [ForeignKey(nameof(UserId))]
    [InverseProperty(nameof(AspNetUser.UserTickets))]
    public virtual AspNetUser User { get; set; } = null!;
}
