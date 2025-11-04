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
    public class GenericService<TEntity, TDto, TKey> : IService<TEntity, TDto, TKey>, IService3DTO<TEntity, TDto, TKey>
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

        public virtual async Task<TDto?> GetAsync(TKey id, CancellationToken ct = default)
        {
            var entity = await _repo.Query()
                                    .AsNoTracking()
                                    .FirstOrDefaultAsync(e => EF.Property<TKey>(e, "Id")!.Equals(id), ct);
            return entity == null ? default : _mapper.Map<TDto>(entity);
        }
        protected virtual IQueryable<TEntity> GetQueryWithIncludes()
        {
            return _repo.Query().AsNoTracking();
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
                var query = GetQueryWithIncludes();

                var projectedQuery = ProjectToDto(query);

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

                return await Application.Common.PagedResult<TDto>.FromQueryableAsync(projectedQuery, page, pageSize, ct);
            }

        protected virtual IQueryable<TEntity> ApplySearch(IQueryable<TEntity> query, string searchQuery)
        {
            return query;
        }

        protected virtual IQueryable<TEntity> ApplyFilter(IQueryable<TEntity> query, string filterField, string filterValue)
        {
            // Tìm property theo tên (không phân biệt hoa thường)
            var prop = typeof(TEntity).GetProperty(
                filterField!,
                System.Reflection.BindingFlags.IgnoreCase |
                System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.Instance
            );

            if (prop == null)
                throw new ArgumentException($"Property '{filterField}' not found on type {typeof(TEntity).Name}");

            // Lấy kiểu dữ liệu thực tế (nếu có nullable thì bỏ nullable wrapper)
            var propType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;

            if (propType == typeof(string))
            {
                // ✅ string: dùng Contains + null-check
                return query.Where($"{filterField} != null && {filterField}.Contains(@0)", filterValue);
            }
            else if (propType == typeof(bool))
            {
                // ✅ bool: parse sang bool và so sánh trực tiếp
                if (!bool.TryParse(filterValue, out var boolVal))
                    throw new ArgumentException($"'{filterValue}' không thể chuyển sang kiểu bool.");
                return query.Where($"{filterField} == @0", boolVal);
            }
            else if (propType.IsEnum)
            {
                // ✅ Enum: parse enum từ string
                var enumValue = Enum.Parse(propType, filterValue, ignoreCase: true);
                return query.Where($"{filterField} == @0", enumValue);
            }
            else
            {
                // ✅ Số hoặc kiểu khác: convert và so sánh bằng ==
                var typedValue = Convert.ChangeType(filterValue, propType);
                return query.Where($"{filterField} == @0", typedValue);
            }
        }
        protected virtual IQueryable<TDto> ProjectToDto(IQueryable<TEntity> query)
             => query.ProjectTo<TDto>(_mapper.ConfigurationProvider);

        protected virtual IQueryable<TEntity> ApplySort(IQueryable<TEntity> query, string? sortOrder)
        {
            if (!string.IsNullOrWhiteSpace(sortOrder))
            {

                var validSortOrder = sortOrder.Replace("_asc", " asc").Replace("_desc", " desc");

                try
                {
                    return query.OrderBy(validSortOrder);
                }
                catch (System.Linq.Dynamic.Core.Exceptions.ParseException)
                {
                    return query.OrderBy("Id");
                }
            }
            else
            {
                return query.OrderBy("Id");
            }
        }
        public virtual async Task<TDto> CreateAsync(TDto dto, CancellationToken ct = default)
        {
            var entity = _mapper.Map<TEntity >(dto);
            await _repo.AddAsync(entity, ct);
            await _uow.SaveChangesAsync(ct);
            return _mapper.Map<TDto>(entity);
        }

        public virtual async Task UpdateAsync(TKey id, TDto dto, CancellationToken ct = default)
        {
            var entity = await _repo.Query().FirstOrDefaultAsync(e => EF.Property<TKey>(e, "Id")!.Equals(id), ct);

            if (entity == null) throw new KeyNotFoundException($"Entity with id {id} not found.");

            _mapper.Map(dto, entity);
            _repo.Update(entity);
            await _uow.SaveChangesAsync(ct);
        }
        public virtual async Task<TDto> CreateAsync<TCreateDto>(TCreateDto dto, CancellationToken ct = default)
        {
            var entity = _mapper.Map<TEntity>(dto);
            await _repo.AddAsync(entity, ct);
            await _uow.SaveChangesAsync(ct);
            return _mapper.Map<TDto>(entity); 
        }

        public virtual async Task UpdateAsync<TUpdateDto>(TKey id, TUpdateDto dto, CancellationToken ct = default)
        {
            var entity = await _repo.Query().FirstOrDefaultAsync(e => EF.Property<TKey>(e, "Id")!.Equals(id), ct)
                        ?? throw new KeyNotFoundException($"Entity with id {id} not found.");
            _mapper.Map(dto, entity); 
            _repo.Update(entity);
            await _uow.SaveChangesAsync(ct);
        }

        public virtual async Task DeleteAsync(TKey id, CancellationToken ct = default)
        {
            var entity = await _repo.Query().FirstOrDefaultAsync(e => EF.Property<TKey>(e, "Id")!.Equals(id), ct);

            if (entity == null) throw new KeyNotFoundException($"Entity with id {id} not found.");

            _repo.Remove(entity);
            await _uow.SaveChangesAsync(ct);
        }
    }
}