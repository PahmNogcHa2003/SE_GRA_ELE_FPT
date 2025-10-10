using Application.Common;
using Application.Interfaces;
using Application.Interfaces.Base;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Services.Base
{
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

        public IQueryable<TEntity> Query() => _repo.Query().AsNoTracking();

        public async Task<TDto?> GetAsync(TKey id, CancellationToken ct = default)
        {
            var entity = await _repo.Query()
                                    .AsNoTracking()
                                    .FirstOrDefaultAsync(e => EF.Property<TKey>(e, "Id")!.Equals(id), ct);
            return entity == null ? default : _mapper.Map<TDto>(entity);
        }

        public async Task<PagedResult<TDto>> GetPagedAsync(
            int page,
            int pageSize,
            Func<IQueryable<TEntity>, IQueryable<TEntity>>? queryShaper = null,
            CancellationToken ct = default)
        {
            var query = _repo.Query().AsNoTracking();

            if (queryShaper != null)
                query = queryShaper(query);

            // Bắt buộc OrderBy để Skip/Take ổn định
            query = query.OrderBy(e => EF.Property<object>(e, "Id"));

            // Project trực tiếp sang DTO
            var projected = query.ProjectTo<TDto>(_mapper.ConfigurationProvider);

            // Phân trang
            return await PagedResult<TDto>.FromQueryableAsync(projected, page, pageSize);
        }

        public async Task<TDto> CreateAsync(TDto dto, CancellationToken ct = default)
        {
            var entity = _mapper.Map<TEntity>(dto);
            await _repo.AddAsync(entity, ct);
            await _uow.SaveChangesAsync(ct);
            return _mapper.Map<TDto>(entity);
        }

        public async Task UpdateAsync(TKey id, TDto dto, CancellationToken ct = default)
        {
            var entity = await _repo.Query()
                                    .FirstOrDefaultAsync(e => EF.Property<TKey>(e, "Id")!.Equals(id), ct);
            if (entity == null) throw new KeyNotFoundException();

            _mapper.Map(dto, entity);
            _repo.Update(entity);
            await _uow.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(TKey id, CancellationToken ct = default)
        {
            var entity = await _repo.Query()
                                    .FirstOrDefaultAsync(e => EF.Property<TKey>(e, "Id")!.Equals(id), ct);
            if (entity == null) throw new KeyNotFoundException();

            _repo.Remove(entity);
            await _uow.SaveChangesAsync(ct);
        }
    }
}
