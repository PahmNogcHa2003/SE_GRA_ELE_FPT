using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces.User.Repository;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.User
{
    public class NewsRepository : BaseRepository<News, long>, INewsRepository
    {
        public NewsRepository(HolaBikeContext dbContext) : base(dbContext)
        {
        }
        public async Task<News?> GetNewsDetailsAsync(long id)
        {
            return await _dbSet.AsNoTracking()
                 .Include(n => n.TagNews)
                    .ThenInclude(tn => tn.Tag)
                .Include(n => n.User)
                .FirstOrDefaultAsync(n => n.Id == id);
        }

        public async Task<News?> GetNewsForUpdateAsync(long id)
        {
            return await _dbSet
                .FirstOrDefaultAsync(n => n.Id == id);
        }
    }
}
