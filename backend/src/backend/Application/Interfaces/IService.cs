using Application.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    // Application/Interfaces/IService.cs
    public interface IService<TEntity, TDto, TKey>
    {
        Task<TDto?> GetAsync(TKey id, CancellationToken ct = default);
        Task<PagedResult<TDto>> GetPagedAsync(int page, int pageSize, CancellationToken ct = default);
        Task<TDto> CreateAsync(TDto dto, CancellationToken ct = default);
        Task UpdateAsync(TKey id, TDto dto, CancellationToken ct = default);
        Task DeleteAsync(TKey id, CancellationToken ct = default);
    }

}
