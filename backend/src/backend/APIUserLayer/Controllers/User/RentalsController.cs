using APIUserLayer.Controllers.Base;
using Application.Common;
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
                return BadRequest(ApiResponse<object>.ErrorResponse(
                    "Invalid rental request",
                    new[] { "Invalid model data" }));

            var result = await _rentalsService.CreateRentalAsync(createRentalDto);

            if (result)
                return Ok(ApiResponse<object>.SuccessResponse(null, "Rental created successfully"));

            return BadRequest(ApiResponse<object>.ErrorResponse("Failed to create rental"));
        }

        /// <summary>
        /// Người dùng kết thúc chuyến thuê xe
        /// </summary>
        [HttpPut("{id}/end")]
        public async Task<IActionResult> EndRental(long id, [FromBody] EndRentalRequestDTO endRentalDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<object>.ErrorResponse(
                    "Invalid rental request",
                    new[] { "Invalid model data" }));

            // Gán rentalId từ route vào DTO (phòng khi client không truyền)
            endRentalDto.RentalId = id;

            var result = await _rentalsService.EndRentalAsync(endRentalDto);

            if (result)
                return Ok(ApiResponse<object>.SuccessResponse(null, "Rental ended successfully"));

            return BadRequest(ApiResponse<object>.ErrorResponse("Failed to end rental"));
        }

        /// <summary>
        /// Lấy thông tin xe bằng mã QR code (BikeCode) và kiểm tra vị trí người dùng.
        /// </summary>
        /// <param name="requestVehicleDTO">Chứa BikeCode, UserLatitude, và UserLongitude.</param>
        /// <returns>VehicleDetailDTO nếu vị trí hợp lệ.</returns>
        [HttpPost("scan-vehicle")]
        public async Task<IActionResult> GetVehicleInfoByScan([FromBody] RequestVehicleDTO requestVehicleDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<object>.ErrorResponse(
                    "Invalid id request",
                    new[] { "Invalid model data" }));

            var vehicleDetail = await _rentalsService.GetVehicleByCode(requestVehicleDTO);

            // Kiểm tra kết quả trả về từ service
            if (vehicleDetail != null)
            {
                // Kiểm tra thành công (Vị trí OK và thông tin xe được tìm thấy)
                return Ok(ApiResponse<VehicleDetailDTO>.SuccessResponse(
                    vehicleDetail,
                    "Vehicle information retrieved and location verified successfully."));
            }

            // Mặc định, nếu service không ném exception mà trả về null (tùy vào cách bạn thiết kế service)
            // thì coi như yêu cầu không hợp lệ hoặc không tìm thấy.
            return NotFound(ApiResponse<object>.ErrorResponse("Không tìm thấy xe hoặc vị trí không hợp lệ."));
        }
    }
}
