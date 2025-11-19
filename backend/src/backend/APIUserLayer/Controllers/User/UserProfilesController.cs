using APIUserLayer.Controllers.Base;
using Application.Common;
using Application.DTOs.UserProfile;
using Application.Interfaces.Photo;
using Application.Interfaces.User.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace APIUserLayer.Controllers.User
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "User")]
    public class UserProfilesController : UserBaseController
    {
        private readonly IUserProfilesService _userProfilesService;
        private readonly IPhotoService _photoService;

        public UserProfilesController(IUserProfilesService userProfilesService, IPhotoService photoService)
        {
            _userProfilesService = userProfilesService;
            _photoService = photoService;
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
        /// <summary>
        /// Upload avatar mới và cập nhật vào UserProfile của user đang đăng nhập
        /// </summary>
        [HttpPost("me/avatar")]
        [ProducesResponseType(typeof(ApiResponse<UserProfileDTO>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        [ProducesResponseType(typeof(ApiResponse<object>), 401)]
        [RequestSizeLimit(5 * 1024 * 1024)]
        public async Task<IActionResult> UploadAvatar(
            IFormFile file,
            CancellationToken ct)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!long.TryParse(userIdString, out var userId))
            {
                return Unauthorized(ApiResponse<object>.ErrorResponse(
                    "Token không hợp lệ hoặc không tìm thấy User ID."));
            }
            if (file == null || file.Length == 0)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(
                    "File avatar không hợp lệ."));
            }
            const long maxFileSizeBytes = 5 * 1024 * 1024;
            if (file.Length > maxFileSizeBytes)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(
                    "Kích thước ảnh vượt quá 5MB. Vui lòng chọn ảnh nhỏ hơn."));
            }
            var allowedContentTypes = new[]
            {
                "image/jpeg",
                "image/png",
                "image/webp",
                "image/jpg"
            };
            if (!allowedContentTypes.Contains(file.ContentType))
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(
                    "Định dạng ảnh không hợp lệ. Chỉ chấp nhận JPEG, PNG, WEBP."));
            }
            var updatedProfile = await _userProfilesService.UpdateAvatarAsync(userId, file, ct);

            if (updatedProfile == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse(
                    "Không tìm thấy hồ sơ người dùng để cập nhật avatar."));
            }
            return Ok(ApiResponse<UserProfileDTO>.SuccessResponse(
                updatedProfile,
                "Cập nhật avatar thành công."));
        }


    }
}
