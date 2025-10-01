using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Models;

[Table("UserTicket")]
public partial class UserTicket
{
    [Key]
    public long Id { get; set; }

    public long UserId { get; set; }

    public long PlanPriceId { get; set; }

    public DateTimeOffset StartTime { get; set; }

    public DateTimeOffset EndTime { get; set; }

    public bool IsUsed { get; set; }

    [InverseProperty("UserTicket")]
    public virtual ICollection<BookingTicket> BookingTickets { get; set; } = new List<BookingTicket>();

    [ForeignKey("PlanPriceId")]
    [InverseProperty("UserTickets")]
    public virtual TicketPlanPrice PlanPrice { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("UserTickets")]
    public virtual AspNetUser User { get; set; } = null!;
}
