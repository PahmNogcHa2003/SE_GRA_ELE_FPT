using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Domain.Entities;

[Table("UserTicket")]
[Microsoft.EntityFrameworkCore.Index(nameof(PlanPriceId), Name = "IX_UserTicket_PlanPriceId")]
[Microsoft.EntityFrameworkCore.Index(nameof(UserId), Name = "IX_UserTicket_UserId")]
public partial class UserTicket : BaseEntity<long>
{
    public long UserId { get; set; }

    public long PlanPriceId { get; set; }

    public DateTimeOffset StartTime { get; set; }

    public DateTimeOffset EndTime { get; set; }

    public bool IsUsed { get; set; }

    [InverseProperty(nameof(BookingTicket.UserTicket))]
    public virtual ICollection<BookingTicket> BookingTickets { get; set; } = new List<BookingTicket>();

    [ForeignKey(nameof(PlanPriceId))]
    [InverseProperty(nameof(TicketPlanPrice.UserTickets))]
    public virtual TicketPlanPrice PlanPrice { get; set; } = null!;

    [ForeignKey(nameof(UserId))]
    [InverseProperty(nameof(AspNetUser.UserTickets))]
    public virtual AspNetUser User { get; set; } = null!;
}
