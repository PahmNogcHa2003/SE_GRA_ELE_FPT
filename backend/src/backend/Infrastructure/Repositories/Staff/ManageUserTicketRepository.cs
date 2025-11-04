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
    public class ManageUserTicketRepository : BaseRepository<UserTicket, long>, IManageUserTicketRepository
    {
        public ManageUserTicketRepository(HolaBikeContext dbContext) : base(dbContext)
        {
        }
    }
}
