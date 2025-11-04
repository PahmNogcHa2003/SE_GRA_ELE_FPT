// Thêm các using cần thiết
using APIUserLayer.Controllers.Base;
using Application.Common;
using Application.DTOs.Contact;
using Application.Interfaces.Staff.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims; // Để lấy StaffId
using System.Threading.Tasks;

namespace AdminLayer.Controllers.Staff
{
    [Route("api/staff/contacts")] // Thay đổi route cho rõ ràng
    [ApiController]
    [Authorize(Roles = "Staff")] // Chỉ định rõ Role
    public class ReplyContactsController : ApiBaseController // Kế thừa từ Base
    {
        // Tiêm (Inject) service chuyên biệt
        private readonly IReplyContactService _replyService;

        public ReplyContactsController(IReplyContactService replyService)
        {
            _replyService = replyService;
        }

        /// <summary>
        /// Staff gửi trả lời cho một liên hệ
        /// </summary>
        /// <param name="id">ID của liên hệ (Contact)</param>
        /// <param name="dto">Nội dung trả lời (ReplyContactDTO)</param>
        /// <returns></returns>
        // ✅ POST: api/staff/contacts/{id}/reply
        [HttpPost("{id:long}/reply")]
        public async Task<IActionResult> ReplyToContact(
            [FromRoute] long id,
            [FromBody] ReplyContactDTO dto)
        {
            // Lấy StaffId từ người dùng đã xác thực (JWT Token)
            var staffIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(staffIdString) || !long.TryParse(staffIdString, out long staffId))
            {
                // Dùng hàm Error() từ ApiBaseController
                return Error("Không thể xác thực nhân viên", null, 401);
            }

            // Gọi service
            await _replyService.ReplyToContactAsync(
                id,
                dto,
                staffId,
                HttpContext.RequestAborted); // Pass CancellationToken

            // Dùng hàm Success() từ ApiBaseController
            return Success<object>(null, "Gửi trả lời thành công");
        }
    }
}