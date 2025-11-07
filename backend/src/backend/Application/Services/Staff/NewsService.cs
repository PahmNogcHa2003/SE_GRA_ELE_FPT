using Application.Common;
using Application.DTOs;
using Application.DTOs.New;
using Application.Interfaces;
using Application.Interfaces.Base;
using Application.Interfaces.Staff.Repository;
using Application.Interfaces.Staff.Service;
using Application.Services.Base;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
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

        // === CÁC PHƯƠNG THỨC CRUD ĐÃ ĐƯỢC TỐI ƯU HÓA ===

        /// <summary>
        /// Lấy chi tiết một bài News, bao gồm cả Author và Tags để hiển thị.
        /// </summary>
        public override async Task<NewsDTO> GetAsync(long id, CancellationToken ct = default)
        {
            var entity = await _newsRepo.GetNewsDetailsAsync(id);
            if (entity == null) throw new KeyNotFoundException($"News with id {id} not found.");
            return _mapper.Map<NewsDTO>(entity);
        }

        /// <summary>
        /// Tạo một bài News mới. Các Tags được liên kết bằng phương pháp Attach để tối ưu hiệu suất.
        /// </summary>
        public override async Task<NewsDTO> CreateAsync(NewsDTO dto, CancellationToken ct = default)
        {
            var entity = _mapper.Map<News>(dto);
            entity.CreatedAt = DateTimeOffset.UtcNow;

            if (dto.TagIds != null && dto.TagIds.Any())
            {
                // Tạo các "stub entity" chỉ chứa Id
                var tagsToAttach = dto.TagIds.Select(tagId => new Tag { Id = tagId }).ToList();

                // Attach chúng vào DbContext. EF sẽ hiểu rằng chúng đã tồn tại và chỉ tạo mối quan hệ.
                foreach (var tag in tagsToAttach)
                {
                    _tagRepo.Attach(tag);
                }           
            }

            await _repo.AddAsync(entity, ct);
            await _uow.SaveChangesAsync(ct);
            return _mapper.Map<NewsDTO>(entity);
        }

        /// <summary>
        /// Cập nhật một bài News. Quan hệ với Tags được xử lý hiệu quả bằng cách xóa liên kết cũ và tạo lại liên kết mới.
        /// </summary>
        public override async Task UpdateAsync(long id, NewsDTO dto, CancellationToken ct = default)
        {
            // Lấy entity gốc kèm theo danh sách Tags hiện tại của nó.
            var entity = await _newsRepo.GetNewsForUpdateAsync(id);
            if (entity == null) throw new KeyNotFoundException($"News with id {id} not found.");

            // Map các thuộc tính đơn giản (Title, Content...) từ DTO sang entity
            _mapper.Map(dto, entity);

            // Xử lý logic cập nhật Tags một cách hiệu quả
            if (dto.TagIds != null)
            {
               
                if (dto.TagIds.Any())
                {
                    // Tạo các "stub entity" chỉ chứa Id cho các tag mới
                    var tagsToAttach = dto.TagIds.Select(tagId => new Tag { Id = tagId }).ToList();

                    // Attach chúng vào DbContext để EF biết chúng đã tồn tại
                    foreach (var tag in tagsToAttach)
                    {
                        _tagRepo.Attach(tag);
                    }

                
                }
            }

            await _uow.SaveChangesAsync(ct);
        }

        // === CÁC PHƯƠNG THỨC APPLY... CHO SEARCH, FILTER, SORT ===

        protected override IQueryable<News> ApplySearch(IQueryable<News> query, string searchQuery)
        {
            if (string.IsNullOrWhiteSpace(searchQuery)) return query;
            var lowerCaseSearchQuery = searchQuery.Trim().ToLower();

            // Câu lệnh này giờ đã an toàn vì query đã được Include Author ở GetPagedAsync
            return query.Where(s =>
                s.User.UserName.ToLower().Contains(lowerCaseSearchQuery) ||
                s.Content.ToLower().Contains(lowerCaseSearchQuery)
            );
        }

        protected override IQueryable<News> ApplyFilter(IQueryable<News> query, string filterField, string filterValue)
        {
            // Logic filter riêng cho 'isactive'
            if (filterField.Equals("isactive", System.StringComparison.OrdinalIgnoreCase))
            {
                if (bool.TryParse(filterValue, out bool isActive))
                {
                    return query.Where(s => s.Status.Equals(isActive));
                }
               
                return query;
            }

            // Với các trường filter khác, gọi về cho base xử lý
            return base.ApplyFilter(query, filterField, filterValue);
        }
    }
}