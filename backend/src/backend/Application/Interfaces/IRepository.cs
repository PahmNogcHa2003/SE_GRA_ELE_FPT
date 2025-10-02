using Application.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    // Application/Interfaces/IRepository.cs
    public interface IRepository<TEntity, TKey> where TEntity : class
    {
        Task<TEntity?> GetByIdAsync(TKey id, CancellationToken ct = default);
        Task<IEnumerable<TEntity>> ListAsync(CancellationToken ct = default);
        Task<PagedResult<TEntity>> PageAsync(int page, int pageSize, CancellationToken ct = default);
        Task AddAsync(TEntity entity, CancellationToken ct = default);
        void Update(TEntity entity);
        void Remove(TEntity entity);
    }

}
