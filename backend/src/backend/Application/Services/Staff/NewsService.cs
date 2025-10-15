using Application.Common;
using Application.DTOs;
using Application.Interfaces;
using Application.Interfaces.Base;
using Application.Interfaces.Staff.Repository;
using Application.Interfaces.Staff.Service;
using Application.Services.Base;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Services.Staff
{
    public class NewsService : GenericService<News, NewsDTO, long>, INewsService
    {
        private readonly INewsRepository _newsRepo;
        private readonly IRepository<Tag, long> _tagRepo;

        public NewsService(
            INewsRepository newsRepo,
            IRepository<Tag, long> tagRepo,
            IMapper mapper,
            IUnitOfWork uow) : base(newsRepo, mapper, uow)
        {
            _newsRepo = newsRepo;
            _tagRepo = tagRepo;
        }

        // === GHI ĐÈ CÁC PHƯƠNG THỨC CRUD ĐỂ XỬ LÝ LOGIC RIÊNG ===

        public override async Task<NewsDTO> GetAsync(long id, CancellationToken ct = default)
        {
            var entity = await _newsRepo.GetNewsDetailsAsync(id);
            if (entity == null) throw new KeyNotFoundException($"News with id {id} not found.");
            return _mapper.Map<NewsDTO>(entity);
        }

        public override async Task<NewsDTO> CreateAsync(NewsDTO dto, CancellationToken ct = default)
        {
            var entity = _mapper.Map<News>(dto);
            entity.CreatedAt = DateTimeOffset.UtcNow;

            if (dto.TagIds != null && dto.TagIds.Any())
            {
                var tags = await _tagRepo.Query().Where(t => dto.TagIds.Contains(t.Id)).ToListAsync(ct);
                entity.Tags = tags;
            }

            await _repo.AddAsync(entity, ct);
            await _uow.SaveChangesAsync(ct);
            return _mapper.Map<NewsDTO>(entity);
        }

        public override async Task UpdateAsync(long id, NewsDTO dto, CancellationToken ct = default)
        {
            var entity = await _newsRepo.GetNewsForUpdateAsync(id);
            if (entity == null) throw new KeyNotFoundException($"News with id {id} not found.");

            _mapper.Map(dto, entity);

            if (dto.TagIds != null)
            {
                var tags = await _tagRepo.Query().Where(t => dto.TagIds.Contains(t.Id)).ToListAsync(ct);
                entity.Tags = tags;
            }

            await _uow.SaveChangesAsync(ct);
        }

        public override async Task DeleteAsync(long id, CancellationToken ct = default)
        {
            var entity = await _newsRepo.GetNewsForUpdateAsync(id);
            if (entity == null) throw new KeyNotFoundException($"News with id {id} not found.");

            _repo.Remove(entity);
            await _uow.SaveChangesAsync(ct);
        }

        // === CÁC PHƯƠNG THỨC APPLY... VẪN GIỮ NGUYÊN ===

        protected override IQueryable<News> ApplySearch(IQueryable<News> query, string searchQuery)
        {
            if (string.IsNullOrWhiteSpace(searchQuery)) return query;
            var lowerCaseSearchQuery = searchQuery.Trim().ToLower();

            // Câu lệnh này giờ đã an toàn và hiệu quả vì query đã được Include Author ở trên
            return query.Where(s =>
                s.Author.UserName.ToLower().Contains(lowerCaseSearchQuery) ||
                s.Content.ToLower().Contains(lowerCaseSearchQuery)
            );
        }

        protected override IQueryable<News> ApplyFilter(IQueryable<News> query, string filterField, string filterValue)
        {
            var lowerFilterField = filterField.ToLower();

            switch (lowerFilterField)
            {
                case "isactive":
                    {
                        if (bool.TryParse(filterValue, out bool isActive))
                        {
                            return query.Where(s => s.IsActive == isActive);
                        }
                        if (filterValue == "1") return query.Where(s => s.IsActive == true);
                        if (filterValue == "0") return query.Where(s => s.IsActive == false);
                        return query;
                    }

                default:
                    return base.ApplyFilter(query, filterField, filterValue);
            }
        }

        protected override IQueryable<News> ApplySort(IQueryable<News> query, string? sortOrder)
        {
            return base.ApplySort(query, sortOrder);
        }
    }
}