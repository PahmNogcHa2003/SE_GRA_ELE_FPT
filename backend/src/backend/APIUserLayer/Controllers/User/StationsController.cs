using Application.Common;
using Application.DTOs;
using Application.Interfaces.Staff.Service;
using Application.Interfaces.User.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace APIUserLayer.Controllers.User
{
    [ApiController]
    [Route("api/[controller]")]
    public class StationsController : ControllerBase
    {
        private readonly Application.Interfaces.User.Service.IStationsService _stations;

        public StationsController(Application.Interfaces.User.Service.IStationsService stations) => _stations = stations;

        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<StationDTO>>>> Get(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null,
            [FromQuery] string? filterField = null,
            [FromQuery] string? filterValue = null,
            [FromQuery] string? sortOrder = null,
            CancellationToken ct = default)
        {
            var data = await _stations.GetPagedAsync(page, pageSize, search, filterField, filterValue, sortOrder, ct);
            return Ok(ApiResponse<PagedResult<StationDTO>>.SuccessResponse(data, "Fetched stations"));
        }

        // Nearby
        [HttpGet("nearby")]
        public async Task<ActionResult<ApiResponse<PagedResult<StationDTO>>>> Nearby(
            [FromQuery] double lat,
            [FromQuery] double lng,
            [FromQuery] double radiusKm = 5,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            CancellationToken ct = default)
        {
            var data = await _stations.GetNearbyPagedAsync(lat, lng, radiusKm, page, pageSize, ct);
            var msg = data.TotalCount == 0 ? "Không có trạm nào trong bán kính yêu cầu" : "Fetched nearby stations";
            return Ok(ApiResponse<PagedResult<StationDTO>>.SuccessResponse(data, msg));
        }
        [HttpGet("all")]
        public async Task<ActionResult<ApiResponse<IEnumerable<StationDTO>>>> All(CancellationToken ct = default)
        {
            var data = await _stations.GetAllAsync(ct);

            return Ok(ApiResponse<IEnumerable<StationDTO>>.SuccessResponse(data, "All stations"));
        }

    }

}
