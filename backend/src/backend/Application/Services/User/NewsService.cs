using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Interfaces.Base;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Application.Services.Base;
using Application.Interfaces.User.Repository;
using Application.Interfaces.User.Service;
using Microsoft.EntityFrameworkCore;
using AutoMapper.QueryableExtensions;

namespace Application.Services.User
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

        // === CÁC PHƯƠNG THỨC APPLY... CHO SEARCH, FILTER, SORT ===

        protected override IQueryable<News> ApplySearch(IQueryable<News> query, string searchQuery)
        {
            if (string.IsNullOrWhiteSpace(searchQuery)) return query;
            var lowerCaseSearchQuery = searchQuery.Trim().ToLower();

            return query.Where(s =>
                (!string.IsNullOrEmpty(s.Title) &&
                 s.Title.ToLower().Contains(lowerCaseSearchQuery)) ||         
                (!string.IsNullOrEmpty(s.Content) &&
                 s.Content.ToLower().Contains(lowerCaseSearchQuery)) ||        
                (s.User != null &&
                 !string.IsNullOrEmpty(s.User.UserName) &&
                 s.User.UserName.ToLower().Contains(lowerCaseSearchQuery))    
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

        public async Task<IEnumerable<NewsDTO>> GetRelatedNewsAsync(
            long newsId,
            int limit = 5,
            CancellationToken ct = default)
        {
            var currentNews = await _newsRepo.Query()
                .Include(n => n.TagNews)
                .ThenInclude(tn => tn.Tag)
                .FirstOrDefaultAsync(n => n.Id == newsId, ct);

            if (currentNews == null)
                throw new KeyNotFoundException($"News with id {newsId} not found.");

            var tagIds = currentNews.TagNews
                .Select(tn => tn.TagId)
                .Distinct()
                .ToList();

            if (!tagIds.Any())
                return Enumerable.Empty<NewsDTO>();

            var query = _newsRepo.Query()
                .Include(n => n.TagNews)
                .ThenInclude(tn => tn.Tag)
                .Where(n => n.Id != newsId &&
                            n.TagNews.Any(tn => tagIds.Contains(tn.TagId)))
                .OrderByDescending(n => n.PublishedAt ?? n.CreatedAt);

            var relatedNews = await query
                .Take(limit)
                .ProjectTo<NewsDTO>(_mapper.ConfigurationProvider)
                .ToListAsync(ct);

            return relatedNews;
        }

    }
}
