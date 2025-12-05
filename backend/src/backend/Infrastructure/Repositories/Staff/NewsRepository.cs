using Application.Common;
using Application.Interfaces.Staff.Repository;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Staff
{
    public class NewsRepository : BaseRepository<News, long>, INewsRepository
    {
        public NewsRepository(HolaBikeContext dbContext) : base(dbContext)
        {
        }
    }
}