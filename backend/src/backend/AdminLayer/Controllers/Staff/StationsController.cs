using Application.Common;
using Application.DTOs.Station;
using Application.Interfaces.Staff.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdminLayer.Controllers.Staff
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class StationsController : ControllerBase
    {
        private readonly IStationsService _stationsService;

        public StationsController(IStationsService stationsService)
        {
            _stationsService = stationsService;
        }
            
        #region Get Paged With Cache
        /// <summary>
        /// Lấy danh sách Station theo trang, có hỗ trợ cache, search/filter/sort
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<StationDTO>>>> GetPaged(
            int page = 1,
            int pageSize = 10,
            string? searchQuery = null,
            string? filterField = null,
            string? filterValue = null,
            string? sortOrder = null)
        {
            var result = await _stationsService.GetPagedAsyncWithCache(
                getCachedData: () => _stationsService.GetAllWithCacheAsyncRaw(),
                refreshCache: () => _stationsService.RefreshCacheAsync(),
                page: page,
                pageSize: pageSize,
                searchQuery: searchQuery,
                filterField: filterField,
                filterValue: filterValue,
                sortOrder: sortOrder
            );

            return Ok(ApiResponse<PagedResult<StationDTO>>.SuccessResponse(result, "Fetched paged stations successfully (cached)"));
        }

        #endregion

        #region Get By Id

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<StationDTO>>> GetStationById(long id)
        {
            var station = await _stationsService.GetAsync(id);
            return Ok(ApiResponse<StationDTO>.SuccessResponse(station, "Fetched station successfully"));
        }

        #endregion

        #region Create

        [HttpPost]
        public async Task<ActionResult<ApiResponse<StationDTO>>> CreateStation([FromBody] StationDTO createDto)
        {
            var createdStation = await _stationsService.CreateAsync(createDto);
            return CreatedAtAction(nameof(GetStationById), new { id = createdStation.Id },
                ApiResponse<StationDTO>.SuccessResponse(createdStation, "Station created successfully"));
        }

        #endregion

        #region Update

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> UpdateStation(long id, [FromBody] StationDTO updateDto)
        {
            if (updateDto.Id != 0 && updateDto.Id != id)
                return BadRequest(ApiResponse<object>.ErrorResponse("Route ID and Body ID do not match", new[] { "Invalid ID parameter" }));

            await _stationsService.UpdateAsync(id, updateDto);
            return Ok(ApiResponse<object>.SuccessResponse(null, "Station updated successfully"));
        }

        #endregion

        #region Delete

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> DeleteStation(long id)
        {
            await _stationsService.DeleteAsync(id);
            return Ok(ApiResponse<object>.SuccessResponse(null, "Station deleted successfully"));
        }

        #endregion

        #region Get All Cached

        [HttpGet("cached")]
        public async Task<ActionResult<ApiResponse<List<StationDTO>>>> GetStationsWithCache()
        {
            var stations = await _stationsService.GetAllWithCacheAsync();
            return Ok(ApiResponse<List<StationDTO>>.SuccessResponse(stations, "Fetched stations with cache successfully"));
        }

        #endregion
    }
}
