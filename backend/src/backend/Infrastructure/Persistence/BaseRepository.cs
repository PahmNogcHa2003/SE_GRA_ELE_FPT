using Application.Common;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Infrastructure.Persistence
{
    public class BaseRepository<TEntity, TKey> : IRepository<TEntity, TKey>
        where TEntity : BaseEntity<TKey>
    {
        protected readonly HolaBikeContext _dbContext;
        protected readonly DbSet<TEntity> _dbSet;

        public BaseRepository(HolaBikeContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = _dbContext.Set<TEntity>();
        }

        public async Task AddAsync(TEntity entity, CancellationToken ct = default)
        {
            await _dbSet.AddAsync(entity, ct);
        }

        public async Task<TEntity?> GetByIdAsync(TKey id, CancellationToken ct = default)
        {
            return await _dbSet.AsNoTracking()
                               .FirstOrDefaultAsync(e => e.Id!.Equals(id), ct);
        }

        public IQueryable<TEntity> Query()
        {
            return _dbSet.AsNoTracking();
        }

        public async Task<IEnumerable<TEntity>> ListAsync(CancellationToken ct = default)
        {
            return await _dbSet.AsNoTracking().ToListAsync(ct);
        }

        public void Update(TEntity entity)
        {
            _dbSet.Update(entity);
        }

        public void Remove(TEntity entity)
        {
            _dbSet.Remove(entity);
        }

        public async Task<PagedResult<TEntity>> PageAsync(int page, int pageSize, CancellationToken ct = default)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;

            var query = _dbSet.AsNoTracking();

            var totalCount = await query.CountAsync(ct);

            var items = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize);

            return new PagedResult<TEntity>(items, totalCount, page, pageSize);
        }
    }
}
