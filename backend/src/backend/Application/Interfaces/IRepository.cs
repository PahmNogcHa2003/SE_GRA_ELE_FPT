using Application.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IRepository<TEntity, TKey> where TEntity : class
    {
        Task<TEntity?> GetByIdAsync(TKey id, CancellationToken ct = default);

        IQueryable<TEntity> Query();

        Task<IEnumerable<TEntity>> ListAsync(CancellationToken ct = default);

        Task<PagedResult<TEntity>> PageAsync(int page, int pageSize, CancellationToken ct = default);

        Task AddAsync(TEntity entity, CancellationToken ct = default);

        void Update(TEntity entity);

        void Remove(TEntity entity);
    }
}
