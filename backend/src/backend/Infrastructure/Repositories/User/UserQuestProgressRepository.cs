using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces.Base;
using Application.Interfaces.User.Repository;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.User
{
    public class UserQuestProgressRepository
        : BaseRepository<UserQuestProgress, long>, IUserQuestProgressRepository
    {
        public UserQuestProgressRepository(HolaBikeContext context) : base(context)
        {
        }

        public async Task<bool> AnyProgressForQuestAsync(long questId, CancellationToken ct = default)
        {
            return await _dbSet
                .AsNoTracking()
                .AnyAsync(p => p.QuestId == questId, ct);
        }
    }
}
