// AdminLayer/Controllers/Staff/StationsController.cs
using Application.Common;
using Application.DTOs;
using Application.Interfaces.Staff.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AdminLayer.Controllers.Staff
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "AdminOnly")]
    public class StationsController : ControllerBase 
    {
        private readonly IStationsService _stationsService;

        public StationsController(IStationsService stationsService)
        {
            _stationsService = stationsService;
        }

        // GET: api/stations
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<StationDTO>>>> GetStations(
            [FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null,
            [FromQuery] string? filterField = null, [FromQuery] string? filterValue = null, [FromQuery] string? sortOrder = null)
        {
            var pagedStations = await _stationsService.GetPagedAsync(page, pageSize, search, filterField, filterValue, sortOrder);
            return Ok(ApiResponse<PagedResult<StationDTO>>.SuccessResponse(pagedStations, "Fetched stations successfully"));
        }

        // GET: api/stations/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<StationDTO>>> GetStationById(long id)
        {
            var station = await _stationsService.GetAsync(id);
            // Service sẽ ném KeyNotFoundException nếu không tìm thấy, Middleware sẽ bắt
            return Ok(ApiResponse<StationDTO>.SuccessResponse(station, "Fetched station successfully"));
        }

        // POST: api/stations
        [HttpPost]
        public async Task<ActionResult<ApiResponse<StationDTO>>> CreateStation([FromBody] StationDTO createDto)
        {
            // ModelState được [ApiController] tự động kiểm tra
            var createdStation = await _stationsService.CreateAsync(createDto);
            var response = ApiResponse<StationDTO>.SuccessResponse(createdStation, "Station created successfully");
            return CreatedAtAction(nameof(GetStationById), new { id = createdStation.Id }, response);
        }

        // PUT: api/stations/5
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> UpdateStation(long id, [FromBody] StationDTO updateDto)
        {
            if (updateDto.Id != 0 && id != updateDto.Id)
            {
                var errorResponse = ApiResponse<object>.ErrorResponse("Route ID and Body ID do not match", new[] { "Invalid ID parameter" });
                return BadRequest(errorResponse);
            }

            await _stationsService.UpdateAsync(id, updateDto);
            return Ok(ApiResponse<object>.SuccessResponse(null, "Station updated successfully"));
        }

        // DELETE: api/stations/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> DeleteStation(long id)
        {
            await _stationsService.DeleteAsync(id);
            return Ok(ApiResponse<object>.SuccessResponse(null, "Station deleted successfully"));
        }
    }
}