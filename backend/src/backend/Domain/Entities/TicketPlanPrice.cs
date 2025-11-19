using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Domain.Entities;

public enum PlanActivationMode { IMMEDIATE = 0, ON_FIRST_USE = 1 }

[Table("TicketPlanPrice")]
[Index(nameof(PlanId), Name = "IX_TicketPlanPrice_PlanId")]
public partial class TicketPlanPrice : BaseEntity<long>
{
    [Required]
    public long PlanId { get; set; }

    [StringLength(50)]
    public string? VehicleType { get; set; }

    [Required]
    [Column(TypeName = "decimal(18, 2)")]
    public decimal Price { get; set; }

    public int? DurationLimitMinutes { get; set; }
    public int? DailyFreeDurationMinutes { get; set; }
    public int? ValidityDays { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? OverageFeePer15Min { get; set; }

    [Required]
    public PlanActivationMode ActivationMode { get; set; } = PlanActivationMode.IMMEDIATE;

    public int? ActivationWindowDays { get; set; } 

    public bool IsActive { get; set; } = true;

    [ForeignKey(nameof(PlanId))]
    [InverseProperty("TicketPlanPrices")]
    public virtual TicketPlan Plan { get; set; } = null!;

    [InverseProperty("PlanPrice")]
    public virtual ICollection<UserTicket> UserTickets { get; set; } = new List<UserTicket>();

    // Constructor giả định để đảm bảo các giá trị cơ bản được thiết lập
    public TicketPlanPrice(long planId, decimal price, PlanActivationMode activationMode)
    {
        if (planId <= 0 || price <= 0)
        {
            throw new ArgumentException("PlanId and Price must be positive.");
        }
        PlanId = planId;
        Price = price;
        ActivationMode = activationMode;
        IsActive = true;
    }

    public TicketPlanPrice() { }

    // 1. Cập nhật giá và phí vượt giờ
    public void UpdatePriceAndFees(decimal newPrice, decimal? newOverageFeePer15Min)
    {
        if (newPrice <= 0)
        {
            throw new ArgumentException("Price must be greater than zero.", nameof(newPrice));
        }
        if (newOverageFeePer15Min.HasValue && newOverageFeePer15Min.Value < 0)
        {
            throw new ArgumentException("Overage fee cannot be negative.", nameof(newOverageFeePer15Min));
        }

        Price = newPrice;
        OverageFeePer15Min = newOverageFeePer15Min;
    }

    // 2. Cập nhật các quy tắc thời hạn/hiệu lực
    public void UpdateDurationRules(int? durationLimitMinutes, int? validityDays, int? activationWindowDays)
    {
        if (durationLimitMinutes.HasValue && durationLimitMinutes.Value <= 0)
        {
            throw new ArgumentException("Duration limit must be positive.", nameof(durationLimitMinutes));
        }
        if (validityDays.HasValue && validityDays.Value <= 0)
        {
            throw new ArgumentException("Validity days must be positive.", nameof(validityDays));
        }
        if (activationWindowDays.HasValue && activationWindowDays.Value <= 0)
        {
            throw new ArgumentException("Activation window days must be positive.", nameof(activationWindowDays));
        }

        DurationLimitMinutes = durationLimitMinutes;
        ValidityDays = validityDays;
        ActivationWindowDays = activationWindowDays;
    }
}
