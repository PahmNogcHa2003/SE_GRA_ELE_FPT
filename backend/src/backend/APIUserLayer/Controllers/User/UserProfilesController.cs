using APIUserLayer.Controllers.Base;
using Application.Common;
using Application.DTOs.UserProfile;
using Application.Interfaces.User.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace APIUserLayer.Controllers.User
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserProfilesController : UserBaseController
    {
        private readonly IUserProfilesService _userProfilesService;

        public UserProfilesController(IUserProfilesService userProfilesService)
        {
            _userProfilesService = userProfilesService;
        }

        /// <summary>
        /// Lấy thông tin UserProfile của user đang đăng nhập
        /// </summary>
        [HttpGet("me")]
        [ProducesResponseType(typeof(ApiResponse<UserProfileDTO>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        public async Task<IActionResult> GetMyProfile()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!long.TryParse(userIdString, out var userId))
                return Unauthorized(ApiResponse<object>.ErrorResponse("Token không hợp lệ hoặc không tìm thấy User ID."));

            var profile = await _userProfilesService.GetByUserIdAsync(userId);
            if (profile == null)
                return NotFound(ApiResponse<object>.ErrorResponse("Không tìm thấy UserProfile cho user này."));

            return Ok(ApiResponse<UserProfileDTO>.SuccessResponse(profile, "Lấy thông tin UserProfile thành công."));
        }

        /// <summary>
        /// Cập nhật hồ sơ của user đang đăng nhập
        /// </summary>
        // APIUserLayer/Controllers/User/UserProfilesController.cs  (trích PUT)
        [HttpPut("me")]
        [ProducesResponseType(typeof(ApiResponse<UserProfileDTO>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        public async Task<IActionResult> UpdateMyProfile([FromBody] UpdateUserProfileBasicDTO dto)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!long.TryParse(userIdString, out var userId))
                return Unauthorized(ApiResponse<object>.ErrorResponse("Token không hợp lệ hoặc không tìm thấy User ID."));

            if (dto == null)
                return BadRequest(ApiResponse<object>.ErrorResponse("Dữ liệu cập nhật không hợp lệ."));

            var updated = await _userProfilesService.UpdateBasicByUserIdAsync(userId, dto);
            if (updated == null)
                return NotFound(ApiResponse<object>.ErrorResponse("Không tìm thấy UserProfile để cập nhật."));

            return Ok(ApiResponse<UserProfileDTO>.SuccessResponse(updated, "Cập nhật hồ sơ thành công."));
        }

    }
}
