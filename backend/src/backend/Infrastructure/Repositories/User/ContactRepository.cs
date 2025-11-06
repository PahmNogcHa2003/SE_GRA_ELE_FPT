using Application.Interfaces.Staff.Repository;
using Application.Interfaces.User.Repository;
using Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.User
{
    public class ContactRepository : BaseRepository<Domain.Entities.Contact, long>, IContactRepository
    {
        public ContactRepository(HolaBikeContext dbContext) : base(dbContext)
        {
        }
    }
}
