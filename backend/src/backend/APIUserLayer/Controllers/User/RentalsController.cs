using APIUserLayer.Controllers.Base;
using Application.Common;
using Application.DTOs;
using Application.Interfaces.Staff.Service;
using Application.Interfaces.User.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace APIUserLayer.Controllers.User
{
    [Route("api/[controller]")]
    [ApiController]
    public class RentalsController : UserBaseController
    {
        private readonly IRentalsService _rentalsService;

        public RentalsController(IRentalsService rentalsService)
        {
            _rentalsService = rentalsService;
        }

        // GET: api/Rentals
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<RentalDTO>>>> GetRentals(
            [FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null,
            [FromQuery] string? filterField = null, [FromQuery] string? filterValue = null, [FromQuery] string? sortOrder = null)
        {
            var pagedRentals = await _rentalsService.GetPagedAsync(page, pageSize, search, filterField, filterValue, sortOrder);
            return Ok(ApiResponse<PagedResult<RentalDTO>>.SuccessResponse(pagedRentals, "Fetched Rentals successfully"));
        }

        // GET: api/Rentals/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<RentalDTO>>> GetStationById(long id)
        {
            var rental = await _rentalsService.GetAsync(id);
            // Service sẽ ném KeyNotFoundException nếu không tìm thấy, Middleware sẽ bắt
            return Ok(ApiResponse<RentalDTO>.SuccessResponse(rental, "Fetched rental successfully"));
        }

        // POST: api/Rentals
        [HttpPost]
        public async Task<ActionResult<ApiResponse<RentalDTO>>> CreateStation([FromBody] RentalDTO createDto)
        {
            // ModelState được [ApiController] tự động kiểm tra
            var createdStation = await _rentalsService.CreateAsync(createDto);
            var response = ApiResponse<RentalDTO>.SuccessResponse(createdStation, "rental created successfully");
            return CreatedAtAction(nameof(GetStationById), new { id = createdStation.Id }, response);
        }

        // PUT: api/Rentals/5
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> UpdateStation(long id, [FromBody] RentalDTO updateDto)
        {
            if (updateDto.Id != 0 && id != updateDto.Id)
            {
                var errorResponse = ApiResponse<object>.ErrorResponse("Route ID and Body ID do not match", new[] { "Invalid ID parameter" });
                return BadRequest(errorResponse);
            }

            await _rentalsService.UpdateAsync(id, updateDto);
            return Ok(ApiResponse<object>.SuccessResponse(null, "rental updated successfully"));
        }

        // DELETE: api/Rentals/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> DeleteStation(long id)
        {
            await _rentalsService.DeleteAsync(id);
            return Ok(ApiResponse<object>.SuccessResponse(null, "rental deleted successfully"));
        }
    }
}
