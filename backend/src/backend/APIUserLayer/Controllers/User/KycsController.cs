using APIUserLayer.Controllers.Base;
using Application.Common;
using Application.DTOs.Kyc; // <-- Quan trọng: Phải using DTO mới
using Application.Interfaces.User.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace APIUserLayer.Controllers.User
{
    [Route("api/[controller]")]
    [ApiController]
    public class KycsController : UserBaseController
    {
        private readonly IKycService _kycService;

        public KycsController(IKycService kycService)
        {
            _kycService = kycService;
        }

        [HttpPost("submit-images")]
        [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        [Authorize]
        // === THAY ĐỔI QUAN TRỌNG NHẤT LÀ Ở ĐÂY ===
        // Phương thức bây giờ nhận vào DTO thay vì 2 tham số IFormFile riêng lẻ.
        public async Task<IActionResult> SubmitKycImages([FromForm] CreateKycRequestDTO request)
        {
            // Lấy id của người dùng từ claims (token)
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !long.TryParse(userIdString, out var userId))
            {
                return Unauthorized(ApiResponse<object>.ErrorResponse("Token không hợp lệ hoặc không tìm thấy User ID."));
            }

            // Gọi service với các file lấy từ DTO
            var result = await _kycService.SubmitKycImagesAsync(userId, request.FrontImage, request.BackImage);

            return Ok(ApiResponse<bool>.SuccessResponse(result, "Yêu cầu đã được gửi. Chúng tôi sẽ xử lý và thông báo cho bạn."));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<KycDTO>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        public async Task<ActionResult<ApiResponse<KycDTO>>> GetKycFormById(long id)
        {
            var kycForm = await _kycService.GetAsync(id);

            if (kycForm == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse($"Không tìm thấy yêu cầu KYC với ID: {id}"));
            }

            return Ok(ApiResponse<KycDTO>.SuccessResponse(kycForm, "Lấy thông tin KYC thành công."));
        }
    }
}

