using Application.DTOs.Station;
using Application.Interfaces;
using Application.Interfaces.Base;
using Application.Interfaces.Cache;
using Application.Interfaces.Photo;
using Application.Interfaces.Staff.Service;
using Application.Services.Base;
using Application.Services.Photo;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Services.Staff
{
    public class StationsService : GenericService<Station, StationDTO, long>, IStationsService
    {
        private readonly IPhotoService _photoService;
        private readonly ICacheService _cacheService;
        private const string CACHE_KEY = "stations:all";

        public StationsService(IPhotoService photoService,
            IRepository<Station, long> repo,
            IMapper mapper,
            IUnitOfWork uow,
            ICacheService cacheService) : base(repo, mapper, uow)
        {
            _photoService = photoService;
            _cacheService = cacheService;
        }

        #region CRUD Overrides để xóa cache

        public override async Task<StationDTO> CreateAsync(StationDTO dto, CancellationToken ct = default)
        {
            var result = await base.CreateAsync(dto, ct);
            await _cacheService.RemoveAsync(CACHE_KEY);
            return result;
        }

        public override async Task UpdateAsync(long id, StationDTO dto, CancellationToken ct = default)
        {
            await base.UpdateAsync(id, dto, ct);
            await _cacheService.RemoveAsync(CACHE_KEY);
        }

        public override async Task DeleteAsync(long id, CancellationToken ct = default)
        {
            await base.DeleteAsync(id, ct);
            await _cacheService.RemoveAsync(CACHE_KEY);
        }

        #endregion

        #region Search / Filter / Sort Overrides

        protected override IQueryable<Station> ApplySearch(IQueryable<Station> query, string searchQuery)
        {
            if (string.IsNullOrWhiteSpace(searchQuery)) return query;

            var lowerCaseSearchQuery = searchQuery.Trim().ToLower();
            return query.Where(s =>
                s.Name.ToLower().Contains(lowerCaseSearchQuery) ||
                s.Location.ToLower().Contains(lowerCaseSearchQuery));
        }

        protected override IQueryable<Station> ApplyFilter(IQueryable<Station> query, string filterField, string filterValue)
        {
            var lowerFilterField = filterField.ToLower();

            switch (lowerFilterField)
            {
                case "isactive":
                    if (bool.TryParse(filterValue, out bool isActive))
                        return query.Where(s => s.IsActive == isActive);
                    if (filterValue == "1") return query.Where(s => s.IsActive);
                    if (filterValue == "0") return query.Where(s => !s.IsActive);
                    return query;

                case "capacitygreaterthan":
                    if (int.TryParse(filterValue, out int capacity))
                        return query.Where(s => s.Capacity > capacity);
                    return query;

                default:
                    return base.ApplyFilter(query, filterField, filterValue);
            }
        }

        protected override IQueryable<Station> ApplySort(IQueryable<Station> query, string? sortOrder)
        {
            return base.ApplySort(query, sortOrder);
        }

        #endregion

        #region Cache Methods

        /// <summary>
        /// Lấy tất cả Station có caching Redis (cache lưu TEntity)
        /// </summary>
        public async Task<List<StationDTO>> GetAllWithCacheAsync()
        {
            var cachedEntities = await _cacheService.GetAsync<List<Station>>(CACHE_KEY);
            if (cachedEntities != null)
                return cachedEntities.Select(e => _mapper.Map<StationDTO>(e)).ToList();

            var data = await _repo.Query()
                                  .Where(s => s.IsActive)
                                  .ToListAsync();

            await _cacheService.SetAsync(CACHE_KEY, data, TimeSpan.FromMinutes(5));

            return data.Select(e => _mapper.Map<StationDTO>(e)).ToList();
        }

        #endregion

        public Task<List<Station>> GetAllWithCacheAsyncRaw() => _cacheService.GetAsync<List<Station>>(CACHE_KEY)
    ?? _repo.Query().Where(s => s.IsActive).ToListAsync();
        
        protected override IQueryable<StationDTO> ProjectToDto(IQueryable<Station> query)
        {
            var readyStatuses = new[] { "Available" };

            return query.Select(s => new StationDTO
            {
                Id = s.Id,
                Name = s.Name,
                Location = s.Location,
                Capacity = s.Capacity,
                Lat = s.Lat,
                Lng = s.Lng,
                IsActive = s.IsActive,
                Image = s.Image,
                VehicleAvailable = s.Vehicles.Count(v => readyStatuses.Contains(v.Status))
            });
        }

        public async Task<StationDTO?> UpdateImageAsync(long stationId, IFormFile imageFile, CancellationToken ct = default)
        {
            if (imageFile == null || imageFile.Length == 0)
                throw new ArgumentException("Image file is null or empty", nameof(imageFile));
            var station = await _repo.Query()
                .FirstOrDefaultAsync(s => s.Id == stationId, ct);
            if (station == null)
                return null;
            if (!string.IsNullOrWhiteSpace(station.ImagePublicId))
            {
                try
                {
                    await _photoService.DeletePhotoAsync(station.ImagePublicId);
                }
                catch
                {
                    Console.WriteLine("Xoá ảnh trạm cũ thất bại, có thể ảnh không tồn tại trên Cloudinary");
                }
            }
            var upload = await _photoService.AddPhotoAsync(imageFile, PhotoPreset.Station);
            if (upload == null || string.IsNullOrWhiteSpace(upload.Url))
                throw new Exception("Upload ảnh trạm thất bại.");
            station.Image = upload.Url;
            station.ImagePublicId = upload.PublicId;
            station.UpdatedAt = DateTimeOffset.UtcNow;

            _repo.Update(station);
            await _uow.SaveChangesAsync(ct);
            return _mapper.Map<StationDTO>(station);
        }

public async Task RefreshCacheAsync()
        {
            var data = await _repo.Query().Where(s => s.IsActive).ToListAsync();
            await _cacheService.SetAsync(CACHE_KEY, data, TimeSpan.FromMinutes(5));
        }

        // Optional: wrapper gọi GenericService method
        public Task<Common.PagedResult<StationDTO>> GetPagedAsyncWithCache(
            Func<Task<List<Station>>> getCachedData,
            Func<Task> refreshCache,
            int page,
            int pageSize,
            string? searchQuery = null,
            string? filterField = null,
            string? filterValue = null,
            string? sortOrder = null
        )
        {
            return base.GetPagedAsyncWithCache(getCachedData, refreshCache, page, pageSize, searchQuery, filterField, filterValue, sortOrder);
        }

    }
}
