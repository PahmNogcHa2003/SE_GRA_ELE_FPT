using Application.Common;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IService<TEntity, TDto, TKey> where TEntity : class
    {
        Task<TDto?> GetAsync(TKey id, CancellationToken ct = default);

        Task<PagedResult<TDto>> GetPagedAsync(
            int page,
            int pageSize,
            Func<IQueryable<TEntity>, IQueryable<TEntity>>? queryShaper = null,
            CancellationToken ct = default);

        Task<TDto> CreateAsync(TDto dto, CancellationToken ct = default);

        Task UpdateAsync(TKey id, TDto dto, CancellationToken ct = default);

        Task DeleteAsync(TKey id, CancellationToken ct = default);
    }
}
