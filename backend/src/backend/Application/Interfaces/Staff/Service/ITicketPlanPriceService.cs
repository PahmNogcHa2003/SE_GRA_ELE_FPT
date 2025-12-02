using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.Tickets;
using Application.Interfaces.Base;
using Domain.Entities;

namespace Application.Interfaces.Staff.Service
{
    public interface ITicketPlanPriceService : IService<TicketPlanPrice, TicketPlanPriceDTO, long>, IService3DTO<TicketPlanPrice, TicketPlanPriceDTO, long>
    {

    }
}
