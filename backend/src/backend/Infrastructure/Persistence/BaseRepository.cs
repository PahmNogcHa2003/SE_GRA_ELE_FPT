using Application.Common;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence
{
    public class BaseRepository<TEntity, TKey> : IRepository<TEntity, TKey>
        where TEntity : BaseEntity<TKey>
    {
        protected readonly AppDbContext _dbContext;
        protected readonly DbSet<TEntity> _dbSet;

        public BaseRepository(AppDbContext dbContext)
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
            return await _dbSet.FirstOrDefaultAsync(e => e.Id!.Equals(id), ct);
        }

        public async Task<IEnumerable<TEntity>> ListAsync(CancellationToken ct = default)
        {
            return await _dbSet.ToListAsync(ct);
        }

        public async Task<PagedResult<TEntity>> PageAsync(int page, int pageSize, CancellationToken ct = default)
        {
            var totalCount = await _dbSet.CountAsync(ct);
            var items = await _dbSet.Skip((page - 1) * pageSize)
                                    .Take(pageSize)
                                    .ToListAsync(ct);

            return new PagedResult<TEntity>(items, totalCount, page, pageSize);
        }

        public void Remove(TEntity entity)
        {
            _dbSet.Remove(entity);
        }

        public void Update(TEntity entity)
        {
            _dbSet.Update(entity);
        }
    }
}
