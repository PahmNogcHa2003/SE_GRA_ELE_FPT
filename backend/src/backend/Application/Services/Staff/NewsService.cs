using Application.DTOs.New;
using Application.DTOs.TagNew;
using Application.Interfaces;
using Application.Interfaces.Base;
using Application.Interfaces.Photo;
using Application.Interfaces.Staff.Repository;
using Application.Interfaces.Staff.Service;
using Application.Services.Base;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace Application.Services.Staff
{
    public class NewsService : GenericService<News, NewsDTO, long>, INewsService
    {
        private readonly INewsRepository _newsRepo;
        private readonly IPhotoService _photoService;
        private readonly IRepository<Tag, long> _tagRepo;
        private readonly IRepository<TagNew, long> _tagNewRepo;
        private readonly ILogger<NewsService> _logger;
        private readonly IUnitOfWork _uow;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public NewsService(
            INewsRepository newsRepo,
            IPhotoService photoService,
            IRepository<Tag, long> tagRepo,
            IRepository<TagNew, long> tagNewRepo,
            IMapper mapper,
            IUnitOfWork uow,
            IHttpContextAccessor httpContextAccessor,
            ILogger<NewsService> logger
        ) : base(newsRepo, mapper, uow)
        {
            _photoService = photoService;
            _newsRepo = newsRepo;
            _tagRepo = tagRepo;
            _tagNewRepo = tagNewRepo;
            _uow = uow;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        private long GetUserId()
        {
            var strId = _httpContextAccessor.HttpContext?
                .User?.FindFirstValue(ClaimTypes.NameIdentifier);

            return long.TryParse(strId, out var id) ? id : 0;
        }

        protected override IQueryable<News> GetQueryWithIncludes()
        {
            return _newsRepo.Query()
                .AsNoTracking()
                .Include(n => n.TagNews)
                    .ThenInclude(tn => tn.Tag)
                .Include(n => n.User);
        }

        // ===========================================
        //                  CREATE
        // ===========================================
        public override async Task<NewsDTO> CreateAsync(NewsDTO dto, CancellationToken ct = default)
        {
            try
            {
                var entity = _mapper.Map<News>(dto);
                entity.CreatedAt = DateTimeOffset.UtcNow;
                entity.UpdatedAt = DateTimeOffset.UtcNow;
                entity.UserId = GetUserId();
                entity.ScheduledAt = dto.ScheduledAt ?? DateTimeOffset.UtcNow;
                entity.PublishedAt = dto.ScheduledAt ?? DateTimeOffset.UtcNow;

                // Insert News
                await _newsRepo.AddAsync(entity, ct);
                await _uow.SaveChangesAsync(ct); // cần có entity.Id

                // Insert Tags
                if (dto.TagIds?.Any() == true)
                {
                    var tagNewsDtos = dto.TagIds.Select(tagId => new TagNewDTO
                    {
                        TagId = tagId,
                        NewId = entity.Id
                    }).ToList();

                    // map list
                    var tagNewsEntities = _mapper.Map<List<TagNew>>(tagNewsDtos);

                    // Add từng tag nếu repo không có AddRangeAsync
                    foreach (var tag in tagNewsEntities)
                    {
                        await _tagNewRepo.AddAsync(tag, ct);
                    }
                }

                await _uow.SaveChangesAsync(ct);
                return _mapper.Map<NewsDTO>(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating news");
                throw;
            }
        }


        // ===========================================
        //                 UPDATE
        // ===========================================
        public override async Task<NewsDTO?> UpdateAsync(long id, NewsDTO dto, CancellationToken ct = default)
        {
            try
            {
                var news = await _newsRepo.GetByIdAsync(dto.Id, ct);
                if (news == null)
                {
                    _logger.LogWarning("News not found: {Id}", dto.Id);
                    return null;
                }

                // Update fields
                news.Title = dto.Title;
                news.Content = dto.Content;
                news.Status = dto.Status;
                news.ScheduledAt = dto.ScheduledAt;
                news.PublishedAt = dto.ScheduledAt ?? news.PublishedAt;
                news.UpdatedAt = DateTimeOffset.UtcNow;
                news.PublishedBy = GetUserId();

                await _uow.SaveChangesAsync(ct);

                return _mapper.Map<NewsDTO>(news);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Update news failed: {Id}", dto.Id);
                throw;
            }
        }

        // ===========================================
        //                 DELETE
        // ===========================================
        public override async Task<NewsDTO?> DeleteAsync(long id, CancellationToken ct = default)
        {
            try
            {
                var news = await _newsRepo.GetByIdAsync(id, ct);
                if (news == null)
                {
                    _logger.LogWarning("News not found: {Id}", id);
                    return null;
                }

                _newsRepo.Remove(news);
                await _uow.SaveChangesAsync(ct);

                return _mapper.Map<NewsDTO>(news);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Delete news failed: {Id}", id);
                throw;
            }
        }

        // ===========================================
        //           BANNER UPDATE (CLEAN)
        // ===========================================
        public async Task<NewsDTO?> UpdateBannerAsync(long newsId, IFormFile file, CancellationToken ct = default)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File banner không hợp lệ");

            var news = await _newsRepo.GetByIdAsync(newsId);
            if (news == null)
                throw new KeyNotFoundException($"News {newsId} not found");

            // Xóa ảnh cũ
            if (!string.IsNullOrWhiteSpace(news.BannerPublicId))
            {
                try
                {
                    await _photoService.DeletePhotoAsync(news.BannerPublicId);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Không xoá được ảnh cũ Banner");
                }
            }

            // Upload ảnh mới
            var upload = await _photoService.AddPhotoAsync(file, PhotoPreset.NewsBanner);
            if (upload == null)
                throw new Exception("Upload banner thất bại");

            news.Banner = upload.Url;
            news.BannerPublicId = upload.PublicId;
            news.UpdatedAt = DateTimeOffset.UtcNow;

            _newsRepo.Update(news);
            await _uow.SaveChangesAsync(ct);

            return _mapper.Map<NewsDTO>(news);
        }

        // ===========================================
        //            SEARCH / FILTER
        // ===========================================
        protected override IQueryable<News> ApplySearch(IQueryable<News> query, string searchQuery)
        {
            if (string.IsNullOrWhiteSpace(searchQuery))
                return query;

            searchQuery = searchQuery.ToLower();

            return query.Where(s =>
                s.User.UserName.ToLower().Contains(searchQuery) ||
                s.Content.ToLower().Contains(searchQuery));
        }

        protected override IQueryable<News> ApplyFilter(IQueryable<News> query, string field, string value)
        {
            if (field.Equals("isactive", StringComparison.OrdinalIgnoreCase))
            {
                if (bool.TryParse(value, out var isActive))
                    return query.Where(s => s.Status == NewStatus.Publish);
            }

            return base.ApplyFilter(query, field, value);
        }
    }
}
