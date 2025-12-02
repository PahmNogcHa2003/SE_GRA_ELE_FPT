using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Base
{
    public interface IService3DTO<TEntity, TReadDto, TKey> where TEntity : class
    {
        Task<TReadDto> CreateAsync<TCreateDto>(TCreateDto dto, CancellationToken ct = default);
        Task UpdateAsync<TUpdateDto>(TKey id, TUpdateDto dto, CancellationToken ct = default);
    }
}
