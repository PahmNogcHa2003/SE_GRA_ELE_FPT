using Application.Interfaces.User.Repository;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.User
{
    public class UserRepository : BaseRepository<AspNetUser, long>, IUserRepository
    {
        public UserRepository(HolaBikeContext ctx) : base(ctx) { }

        // Lấy user theo email
        public Task<AspNetUser?> GetByEmailAsync(string email, CancellationToken ct = default)
        {
            return _dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == email, ct);
        }     

        public IQueryable<AspNetUser> QueryAllUserByStatus()
        {
            var query = _dbSet.AsNoTracking();

            
            return query.OrderByDescending(u => u.Id);
        }
    }
}