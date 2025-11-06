using Application.Interfaces.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Staff.Repository
{
    public interface IReplyContactRepository : IRepository<Domain.Entities.Contact, long>
    {
        Task<bool> IsContactRepliedAsync(long id, CancellationToken cancellationToken = default);
    }
}
