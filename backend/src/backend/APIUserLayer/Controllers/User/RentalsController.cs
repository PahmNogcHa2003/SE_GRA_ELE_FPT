using APIUserLayer.Controllers.Base;
using Application.Common;
using Application.DTOs.Rental;
using Application.DTOs.RentalHistory;
using Application.Interfaces.User.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Application.Services.Identity;

namespace APIUserLayer.Controllers.User
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RentalsController : UserBaseController
    {
        private readonly IRentalsService _rentalsService;
        private readonly IUserLifetimeStatsService _userLifetimeStatsService;
        public RentalsController(IRentalsService rentalsService, IUserLifetimeStatsService userLifetimeStatsService )
        {
            _rentalsService = rentalsService;
            _userLifetimeStatsService = userLifetimeStatsService;
        }

        /// <summary>
        /// Người dùng bắt đầu thuê xe
        /// </summary>
        [HttpPost("start")]
        public async Task<IActionResult> CreateRental([FromBody] CreateRentalDTO createRentalDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToArray();

                return BadRequest(ApiResponse<object>.ErrorResponse(
                    "Yêu cầu thuê xe không hợp lệ.", 
                    errors.Length > 0 ? errors : new[] { "Dữ liệu không hợp lệ." } 
                ));
            }

            var rentalId = await _rentalsService.CreateRentalAsync(createRentalDto);
            return Ok(ApiResponse<long>.SuccessResponse(rentalId, "Tạo lượt thuê thành công."));
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
        ///<summary>
        ///Lấy thông tin lịch sử đi xe của người dùng
        ///<summary>
        [HttpGet("history")]
        public async Task<ActionResult<IEnumerable<RentalHistoryDTO>>> GetMyHistory(CancellationToken ct)
        {
            var list = await _rentalsService.GetMyRentalHistoryAsync(ct);
            return Ok(ApiResponse<IEnumerable<RentalHistoryDTO>>.SuccessResponse(list, "Fetched rental history successfully."));
        }

        /// <summary>
        /// Lấy thống kê quãng đường, số phút đi, calories của người dùng
        /// </summary>
        /// <param name="ct"></param>
        /// <returns></returns>
        [HttpGet("stats/summary")]
        public async Task<IActionResult> GetRentalSummaryStats(CancellationToken ct)
        {
           var userId = ClaimsPrincipalExtensions.GetUserIdAsLong(User);
           var stats = await _userLifetimeStatsService.GetMyStatsAsync(userId, ct);
           return Ok(ApiResponse<RentalStatsSummaryDTO>.SuccessResponse(stats, "Fetched rental summary stats successfully."));
        }
      
    }
}
