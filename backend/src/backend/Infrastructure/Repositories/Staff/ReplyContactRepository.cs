using Application.Interfaces.Staff.Repository;
using Domain.Entities;
using Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Staff
{
    public class ReplyContactRepository : BaseRepository<Contact, long>, IReplyContactRepository
    {
        public ReplyContactRepository(HolaBikeContext dbContext) : base(dbContext)
        {
        }

        public Task<bool> IsContactRepliedAsync(long id, CancellationToken cancellationToken = default)
        {
            try
            {

            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
    }
}
