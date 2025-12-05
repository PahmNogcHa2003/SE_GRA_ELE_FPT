using Application.Common;
using Application.DTOs.Rental.Manage;
using Application.Interfaces.Staff.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AdminLayer.Controllers.Staff
{
    [Route("api/[controller]")]
    [ApiController]
    public class RentalController : ControllerBase
    {
        private readonly IManageRentalsService _manageRentalsService;
        public RentalController(IManageRentalsService manageRentalsService)
        {
            _manageRentalsService = manageRentalsService;
        }
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<RentalListDTO>>>> GetPaged(
          [FromQuery] int page = 1,
          [FromQuery] int pageSize = 20,
          [FromQuery] string? status = null,
          [FromQuery] long? userId = null,
          [FromQuery] long? vehicleId = null,
          [FromQuery] long? startStationId = null,
          [FromQuery] long? endStationId = null,
          [FromQuery] DateTimeOffset? fromStart = null,
          [FromQuery] DateTimeOffset? toStart = null,
          [FromQuery] DateTimeOffset? fromEnd = null,
          [FromQuery] DateTimeOffset? toEnd = null,
          [FromQuery] string? keyword = null,
          CancellationToken ct = default)
        {
            var filter = new RentalFilterDTO
            {
                Status = status,
                UserId = userId,
                VehicleId = vehicleId,
                StartStationId = startStationId,
                EndStationId = endStationId,
                FromStartTimeUtc = fromStart,
                ToStartTimeUtc = toStart,
                FromEndTimeUtc = fromEnd,
                ToEndTimeUtc = toEnd,
                Keyword = keyword
            };

            var paged = await _manageRentalsService.GetPagedAsync(page, pageSize, filter, ct);
            return Ok(ApiResponse<PagedResult<RentalListDTO>>.SuccessResponse(paged, "Danh sách rentals."));
        }
        [HttpGet("{id:long}")]
        public async Task<ActionResult<ApiResponse<RentalDetailDTO>>> GetDetail(long id, CancellationToken ct)
        {
            var dto = await _manageRentalsService.GetDetailAsync(id, ct);
            return Ok(ApiResponse<RentalDetailDTO>.SuccessResponse(dto, "Chi tiết rental."));
        }
    }
}
