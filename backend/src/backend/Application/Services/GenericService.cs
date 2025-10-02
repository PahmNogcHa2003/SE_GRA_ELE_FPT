using Application.Common;
using Application.Interfaces;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    // Application/Services/GenericService.cs
    public class GenericService<TEntity, TDto, TKey> : IService<TEntity, TDto, TKey>
     where TEntity : class
    {
        protected readonly IRepository<TEntity, TKey> _repo;
        protected readonly IMapper _mapper;
        protected readonly IUnitOfWork _uow;

        public GenericService(IRepository<TEntity, TKey> repo, IMapper mapper, IUnitOfWork uow)
        {
            _repo = repo;
            _mapper = mapper;
            _uow = uow;
        }

        public async Task<TDto?> GetAsync(TKey id, CancellationToken ct = default)
        {
            var e = await _repo.GetByIdAsync(id, ct);
            return e == null ? default : _mapper.Map<TDto>(e);
        }

        public async Task<PagedResult<TDto>> GetPagedAsync(int page, int pageSize, CancellationToken ct = default)
        {
            var p = await _repo.PageAsync(page, pageSize, ct);
            return new PagedResult<TDto>
            {
                Items = p.Items.Select(i => _mapper.Map<TDto>(i)),
                TotalCount = p.TotalCount,
                Page = p.Page,
                PageSize = p.PageSize
            };
        }

        public async Task<TDto> CreateAsync(TDto dto, CancellationToken ct = default)
        {
            var entity = _mapper.Map<TEntity>(dto);
            await _repo.AddAsync(entity, ct);
            await _uow.SaveChangesAsync(ct);
            return _mapper.Map<TDto>(entity); // may include assigned Id
        }

        public async Task UpdateAsync(TKey id, TDto dto, CancellationToken ct = default)
        {
            var existing = await _repo.GetByIdAsync(id, ct);
            if (existing == null) throw new KeyNotFoundException();
            _mapper.Map(dto, existing);
            _repo.Update(existing);
            await _uow.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(TKey id, CancellationToken ct = default)
        {
            var existing = await _repo.GetByIdAsync(id, ct);
            if (existing == null) throw new KeyNotFoundException();
            _repo.Remove(existing);
            await _uow.SaveChangesAsync(ct);
        }
    }

}
