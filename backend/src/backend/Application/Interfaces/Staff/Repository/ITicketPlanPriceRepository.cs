using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces.Base;

namespace Application.Interfaces.Staff.Repository
{
    public interface ITicketPlanPriceRepository : IRepository<Domain.Entities.TicketPlanPrice, long>
    {

    }
}
