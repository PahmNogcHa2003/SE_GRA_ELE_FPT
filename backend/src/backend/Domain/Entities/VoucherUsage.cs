using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

[Table("VoucherUsage")]
public class VoucherUsage : BaseEntity<long>
{
    public long VoucherId { get; set; }
    public long UserId { get; set; }
    public long? TicketPlanPriceId { get; set; }
    public DateTimeOffset UsedAt { get; set; }

    [ForeignKey(nameof(VoucherId))]
    public virtual Voucher Voucher { get; set; } = null!;

    [ForeignKey(nameof(UserId))]
    public virtual AspNetUser User { get; set; } = null!;

    [ForeignKey(nameof(TicketPlanPriceId))]
    public virtual TicketPlanPrice? TicketPlanPrice { get; set; }
}
