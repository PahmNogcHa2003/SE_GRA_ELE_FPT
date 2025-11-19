using Application.DTOs;
using Application.DTOs.Station;
using Application.Interfaces;
using Application.Interfaces.Base;
using Application.Interfaces.Photo;
using Application.Interfaces.Staff.Service;
using Application.Services.Base;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Application.Services.Staff
{
    public class StationsService : GenericService<Station, StationDTO, long>, IStationsService
    {
        private readonly IPhotoService _photoService;
        public StationsService(IPhotoService photoService,IRepository<Station, long> repo, IMapper mapper, IUnitOfWork uow) : base(repo, mapper, uow)
        {
            _photoService = photoService;
        }

        /// <summary>
        /// Overrides the generic search logic.
        /// Searches within the Name and Location properties of a Station.
        /// </summary>
        protected override IQueryable<Station> ApplySearch(IQueryable<Station> query, string searchQuery)
        {
            if (string.IsNullOrWhiteSpace(searchQuery))
            {
                return query;
            }

            var lowerCaseSearchQuery = searchQuery.Trim().ToLower();

            return query.Where(s =>
                s.Name.ToLower().Contains(lowerCaseSearchQuery) ||
                s.Location.ToLower().Contains(lowerCaseSearchQuery)
            );
        }

        /// <summary>
        /// Overrides the detailed filtering logic.
        /// Allows filtering based on custom business rules specific to Stations.
        /// </summary>
        protected override IQueryable<Station> ApplyFilter(IQueryable<Station> query, string filterField, string filterValue)
        {
            var lowerFilterField = filterField.ToLower();

            switch (lowerFilterField)
            {
                // Filter by exact Status (supports "true"/"false" and "1"/"0")
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

                // Filter for stations with capacity greater than a specific value
                case "capacitygreaterthan":
                    if (int.TryParse(filterValue, out int capacity))
                    {
                        return query.Where(s => s.Capacity > capacity);
                    }
                    return query; // Skip if the value is not a valid integer

                // Default: fall back to the base class logic (uses .Contains() for other fields)
                default:
                    return base.ApplyFilter(query, filterField, filterValue);
            }
        }

        /// <summary>
        /// Ghi đè logic sắp xếp.
        /// Chỉ xử lý các quy tắc sắp xếp đặc biệt dành riêng cho Station.
        /// Các trường hợp còn lại sẽ được chuyển về cho lớp GenericService xử lý.
        /// </summary>
        protected override IQueryable<Station> ApplySort(IQueryable<Station> query, string? sortOrder)
        {
            var lowerSortOrder = sortOrder?.Trim().ToLower();

            switch (lowerSortOrder)
            {
                default:
                    return base.ApplySort(query, sortOrder);
            }
        }
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
            if(imageFile == null || imageFile.Length == 0)
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
            if(upload == null || string.IsNullOrWhiteSpace(upload.Url))
                throw new Exception("Upload ảnh trạm thất bại.");
            station.Image = upload.Url;
            station.ImagePublicId = upload.PublicId;
            station.UpdatedAt = DateTimeOffset.UtcNow;

            _repo.Update(station);
            await _uow.SaveChangesAsync(ct);
            return _mapper.Map<StationDTO>(station);
        }
    }
}