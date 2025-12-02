using Application.DTOs.Station;
using Application.Interfaces.Base;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Staff.Service
{
    public interface IStationsService : IService<Domain.Entities.Station, StationDTO, long>
    {
        Task<StationDTO?> UpdateImageAsync(long stationId, IFormFile file, CancellationToken ct = default);
        // Lấy tất cả station từ cache dưới dạng DTO
        Task<List<StationDTO>> GetAllWithCacheAsync();

        // Lấy danh sách station raw (TEntity) cho cache/paging
        Task<List<Station>> GetAllWithCacheAsyncRaw();

        // Refresh cache
        Task RefreshCacheAsync();

        // Nếu muốn, khai báo luôn GetPagedAsyncWithCache để Controller gọi được
        Task<Common.PagedResult<StationDTO>> GetPagedAsyncWithCache(
            Func<Task<List<Station>>> getCachedData,
            Func<Task> refreshCache,
            int page,
            int pageSize,
            string? searchQuery = null,
            string? filterField = null,
            string? filterValue = null,
            string? sortOrder = null
        );
    }
}
