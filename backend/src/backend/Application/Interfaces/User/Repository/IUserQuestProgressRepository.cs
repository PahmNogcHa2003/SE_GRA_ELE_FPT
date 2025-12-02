using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces.Base;
using Domain.Entities;

namespace Application.Interfaces.User.Repository
{
    public interface IUserQuestProgressRepository : IRepository<UserQuestProgress, long>
    {
        Task<bool> AnyProgressForQuestAsync(long questId, CancellationToken ct = default);
    }
}
