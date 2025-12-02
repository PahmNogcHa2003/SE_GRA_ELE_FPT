using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Interfaces.Base;
using Application.Interfaces;
using Application.Services.Base;
using AutoMapper;
using Domain.Entities;
using Application.Interfaces.User.Service;
using Microsoft.EntityFrameworkCore;
using Application.Common;
using AutoMapper.QueryableExtensions;
using Application.DTOs.Station;

namespace Application.Services.User
{
    public class StationsService : GenericService<Station, StationDTO, long>, IStationsService
    {
        public StationsService(IRepository<Station, long> repo, IMapper mapper, IUnitOfWork uow) : base(repo, mapper, uow)
        {
        }
        protected override IQueryable<Station> GetQueryWithIncludes()
       => _repo.Query().AsNoTracking().Include(s => s.Vehicles);

        // Search theo Name/Location
        protected override IQueryable<Station> ApplySearch(IQueryable<Station> query, string searchQuery)
        {
            var q = searchQuery.Trim().ToLower();
            return query.Where(s => s.Name.ToLower().Contains(q) || (s.Location ?? "").ToLower().Contains(q));
        }

        // Filter thêm isActive/capacity…
        protected override IQueryable<Station> ApplyFilter(IQueryable<Station> query, string field, string value)
        {
            switch (field.ToLower())
            {
                case "isactive":
                    if (bool.TryParse(value, out var b)) return query.Where(s => s.IsActive == b);
                    if (value == "1") return query.Where(s => s.IsActive);
                    if (value == "0") return query.Where(s => !s.IsActive);
                    return query;
                case "capacitygreaterthan":
                    if (int.TryParse(value, out var c)) return query.Where(s => s.Capacity > c);
                    return query;
                default:
                    return base.ApplyFilter(query, field, value);
            }
        }
        protected override IQueryable<StationDTO> ProjectToDto(IQueryable<Station> query)
        {
            var readyStatuses = new[] { "Available"};

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

        // ====== Hàm chuyên biệt: Nearby (Haversine trong RAM) ======
        public async Task<PagedResult<StationDTO>> GetNearbyPagedAsync(
            double lat, double lng, double radiusKm, int page, int pageSize, CancellationToken ct = default)
        {
            const double R = 6371.0;

            // bounding box
            double latDelta = (radiusKm / R) * (180.0 / Math.PI);
            double lngDelta = (radiusKm / (R * Math.Cos(lat * Math.PI / 180.0))) * (180.0 / Math.PI);
            double minLat = lat - latDelta, maxLat = lat + latDelta;
            double minLng = lng - lngDelta, maxLng = lng + lngDelta;

            // rút ứng viên từ DB
            var candidates = await _repo.Query().AsNoTracking()
                .Include(s => s.Vehicles) // nếu muốn giữ VehicleAvailable
                .Where(s => s.IsActive
                    && s.Lat >= (decimal)minLat && s.Lat <= (decimal)maxLat
                    && s.Lng >= (decimal)minLng && s.Lng <= (decimal)maxLng)
                .Select(s => new
                {
                    s.Id,
                    s.Name,
                    s.Location,
                    s.Capacity,
                    s.Lat,
                    s.Lng,
                    s.IsActive,
                    s.Image,
                    VehicleAvailable = s.Vehicles.Count(v => v.Status == "Available")
                })
                .ToListAsync(ct);

            static double Hav(double a1, double o1, double a2, double o2)
            {
                const double RR = 6371.0;
                double dA = (a2 - a1) * Math.PI / 180.0;
                double dO = (o2 - o1) * Math.PI / 180.0;
                double A = Math.Sin(dA / 2) * Math.Sin(dA / 2) +
                           Math.Cos(a1 * Math.PI / 180.0) * Math.Cos(a2 * Math.PI / 180.0) *
                           Math.Sin(dO / 2) * Math.Sin(dO / 2);
                return RR * 2 * Math.Atan2(Math.Sqrt(A), Math.Sqrt(1 - A));
            }

            var withDist = candidates
                .Select(s =>
                {
                    var d = Hav((double)s.Lat, (double)s.Lng, lat, lng);
                    return new StationDTO
                    {
                        Id = s.Id,
                        Name = s.Name,
                        Location = s.Location,
                        Capacity = s.Capacity,
                        Lat = s.Lat,
                        Lng = s.Lng,
                        IsActive = s.IsActive,
                        Image = s.Image,
                        VehicleAvailable = s.VehicleAvailable,
                        DistanceKm = d
                    };
                })
                .Where(dto => dto.DistanceKm <= radiusKm)
                .OrderBy(dto => dto.DistanceKm);

            var total = withDist.Count();
            var items = withDist.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            return new PagedResult<StationDTO>(items, total, page, pageSize);
        }

        public async Task<IEnumerable<StationDTO>> GetAllAsync(CancellationToken ct = default)
        {
            var query = GetQueryWithIncludes();
            
            //  Hàm này đã có logic đếm s.Vehicles.Count(v => v.Status == "Available")
            var dtoQuery = ProjectToDto(query);

            //  Thực thi query và trả về kết quả
            return await dtoQuery.ToListAsync(ct);
        }

    }
}
