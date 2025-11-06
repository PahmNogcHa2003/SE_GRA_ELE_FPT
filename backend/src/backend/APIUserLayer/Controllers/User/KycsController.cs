using APIUserLayer.Controllers.Base;
using Application.Common;
using Application.DTOs.Kyc;
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

        /// <summary>
        /// Submit ảnh CCCD để tạo yêu cầu KYC
        /// </summary>
        /// <param name="request">DTO chứa FrontImage, BackImage và JsonData</param>
        [HttpPost("submit-images")]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        public async Task<IActionResult> SubmitKycImages([FromForm] CreateKycRequestDTO request)
        {
            if (request == null)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("Request không hợp lệ."));
            }

            var result = await _kycService.CreateKycAsync(request);

            if (!result)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("Tạo yêu cầu KYC thất bại. Vui lòng kiểm tra lại thông tin."));
            }

            return Ok(ApiResponse<bool>.SuccessResponse(true, "Yêu cầu KYC đã được gửi. Chúng tôi sẽ xử lý và thông báo cho bạn."));
        }
    }
}
