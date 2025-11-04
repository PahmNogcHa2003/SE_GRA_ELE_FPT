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
    public interface IManageTicketPlanService
        : IService<TicketPlan, TicketPlanDTO, long>,
          IService3DTO<TicketPlan, TicketPlanDTO, long>
    {

    }
}
