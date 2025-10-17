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

    [Required]
    [StringLength(20)]
    [Unicode(false)]
    public string Status { get; set; } = "Ready";

    [Precision(0)]
    public DateTimeOffset? ActivatedAt { get; set; }

    [Precision(0)]
    public DateTimeOffset? ExpiresAt { get; set; }

    public int? RemainingMinutes { get; set; }

    public int? RemainingRides { get; set; }

    [Required]
    [Precision(0)]
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
