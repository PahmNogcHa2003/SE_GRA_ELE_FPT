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
}
