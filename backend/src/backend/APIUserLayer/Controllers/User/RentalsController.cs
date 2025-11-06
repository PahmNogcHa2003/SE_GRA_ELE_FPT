using APIUserLayer.Controllers.Base;
using Application.Common;
using Application.DTOs;
using Application.DTOs.Rental;
using Application.Interfaces.User.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace APIUserLayer.Controllers.User
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] 
    public class RentalsController : UserBaseController
    {
        private readonly IRentalsService _rentalsService;

        public RentalsController(IRentalsService rentalsService)
        {
            _rentalsService = rentalsService;
        }

        /// <summary>
        /// Người dùng bắt đầu thuê xe
        /// </summary>
        [HttpPost("start")]
        public async Task<IActionResult> CreateRental([FromBody] CreateRentalDTO createRentalDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<object>.ErrorResponse("Invalid rental request", new[] { "Invalid model data" }));

            var result = await _rentalsService.CreateRentalAsync(createRentalDto);

            if (result)
                return Ok(ApiResponse<object>.SuccessResponse(null, "Rental created successfully"));

            return BadRequest(ApiResponse<object>.ErrorResponse("Failed to create rental"));
        }

        /// <summary>
        /// Người dùng kết thúc chuyến thuê xe
        /// (sẽ xử lý sau trong EndRentalAsync)
        /// </summary>
        [HttpPut("{id}/end")]
        public async Task<IActionResult> EndRental(long id, [FromBody] EndRentalRequestDTO endRentalDto)
        {
            await _rentalsService.EndRentalAsync(endRentalDto);
            return Ok(ApiResponse<object>.SuccessResponse(null, "Rental ended successfully"));
        }
    }
}
