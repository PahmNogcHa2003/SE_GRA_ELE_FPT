
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    // Infrastructure/Repositories/UserRepository.cs
    public class UserRepository : BaseRepository<AspNetUser, long>
    {
        public UserRepository(HolaBikeContext ctx) : base(ctx) { }

        // custom query
        public Task<AspNetUser?> GetByEmailAsync(string email, CancellationToken ct = default) =>
            _dbSet.OfType<AspNetUser>().FirstOrDefaultAsync(u => u.Email == email, ct);
    }

}
