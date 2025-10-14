using Application.Common;
using Application.Interfaces;
using Application.Interfaces.Base;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
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

        public async Task<TDto?> GetAsync(TKey id, CancellationToken ct = default)
        {
            var entity = await _repo.Query()
                                    .AsNoTracking()
                                    .FirstOrDefaultAsync(e => EF.Property<TKey>(e, "Id")!.Equals(id), ct);
            return entity == null ? default : _mapper.Map<TDto>(entity);
        }

        public async Task<Application.Common.PagedResult<TDto>> GetPagedAsync(
         int page,
         int pageSize,
         string? searchQuery = null,
         string? filterField = null,
         string? filterValue = null,
         string? sortOrder = null,
         CancellationToken ct = default)
            {
                var query = _repo.Query().AsNoTracking();

                // 1. Apply Search (Search)
                if (!string.IsNullOrWhiteSpace(searchQuery))
                {
                    query = ApplySearch(query, searchQuery);
                }

                // 2. Apply Filter (Filter)
                if (!string.IsNullOrWhiteSpace(filterField) && !string.IsNullOrWhiteSpace(filterValue))
                {
                    query = ApplyFilter(query, filterField, filterValue);
                }

                // 3. Apply Sort (Sort)
                query = ApplySort(query, sortOrder);

                var projectedQuery = query.ProjectTo<TDto>(_mapper.ConfigurationProvider);

                return await Application.Common.PagedResult<TDto>.FromQueryableAsync(projectedQuery, page, pageSize, ct);
            }

        protected virtual IQueryable<TEntity> ApplySearch(IQueryable<TEntity> query, string searchQuery)
        {
            return query;
        }

        protected virtual IQueryable<TEntity> ApplyFilter(IQueryable<TEntity> query, string filterField, string filterValue)
        {
            return query.Where($"{filterField}.Contains(@0)", filterValue);
        }

        protected virtual IQueryable<TEntity> ApplySort(IQueryable<TEntity> query, string? sortOrder)
        {
            if (!string.IsNullOrWhiteSpace(sortOrder))
            {
                return query.OrderBy(sortOrder);
            }
            else
            {
                return query.OrderBy("Id");
            }
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
            var entity = await _repo.Query().FirstOrDefaultAsync(e => EF.Property<TKey>(e, "Id")!.Equals(id), ct);

            if (entity == null) throw new KeyNotFoundException($"Entity with id {id} not found.");

            _mapper.Map(dto, entity);
            _repo.Update(entity);
            await _uow.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(TKey id, CancellationToken ct = default)
        {
            var entity = await _repo.Query().FirstOrDefaultAsync(e => EF.Property<TKey>(e, "Id")!.Equals(id), ct);

            if (entity == null) throw new KeyNotFoundException($"Entity with id {id} not found.");

            _repo.Remove(entity);
            await _uow.SaveChangesAsync(ct);
        }
    }
}