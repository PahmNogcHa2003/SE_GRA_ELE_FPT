using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

[Table("Voucher")]
public class Voucher : BaseEntity<long>
{
    [Required]
    [StringLength(50)]
    public string Code { get; set; } = null!;

    // % hoặc số tiền giảm trực tiếp
    [Required]
    public bool IsPercentage { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Value { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? MaxDiscountAmount { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? MinOrderAmount { get; set; }

    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset EndDate { get; set; }

    // Tổng số lần voucher được dùng
    public int? UsageLimit { get; set; }

    // Mỗi user dùng tối đa bao nhiêu lần
    public int? UsagePerUser { get; set; }

    public string Status { get; set; }

    // --- Navigation ---
    [InverseProperty("Voucher")]
    public virtual ICollection<TicketPlanPrice> TicketPlanPrices { get; set; } = new List<TicketPlanPrice>();

    [InverseProperty("Voucher")]
    public virtual ICollection<VoucherUsage> VoucherUsages { get; set; } = new List<VoucherUsage>();
}
