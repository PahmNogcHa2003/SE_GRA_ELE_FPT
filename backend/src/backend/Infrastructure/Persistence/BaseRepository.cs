using Application.Common;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Application.Interfaces.Base;

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
            // Đánh dấu entity là đã thay đổi.
            // DbContext sẽ theo dõi nó và lưu khi UnitOfWork được commit.
            _dbSet.Update(entity);
        }

        public void Remove(TEntity entity)
        {
            _dbSet.Remove(entity);
        }

        // SỬA LẠI PHƯƠNG THỨC NÀY
        public async Task<PagedResult<TEntity>> PageAsync(int page, int pageSize, CancellationToken ct = default)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;

            var query = _dbSet.AsNoTracking();

            var totalCount = await query.CountAsync(ct);

            // THÊM: Bắt buộc phải sắp xếp trước khi dùng Skip/Take
            var items = await query
                .OrderBy(e => e.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct); // SỬA: Thực thi query để lấy dữ liệu

            return new PagedResult<TEntity>(items, totalCount, page, pageSize);
        }
    }
}