using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces.Staff.Repository;
using Domain.Entities;
using Infrastructure.Persistence;

namespace Infrastructure.Repositories.Staff
{
    internal class TicketPlanPriceRepository : BaseRepository<TicketPlanPrice, long>, ITicketPlanPriceRepository
    {
        public TicketPlanPriceRepository(HolaBikeContext dbContext) : base(dbContext)
        {
        }
    }
}
