using Application.Common;
using Application.DTOs.Quest;
using Application.Interfaces.User.Service;
using Application.Services.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace APIUserLayer.Controllers.User
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestController : ControllerBase
    {
        private readonly IQuestService _questService;
        public QuestController(IQuestService questService)
        {
            _questService = questService;
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetMyActiveQuests(CancellationToken ct)
        {
            var userId = ClaimsPrincipalExtensions.GetUserIdAsLong(User);
            var activeQuests = await _questService.GetMyActiveQuestsAsync(userId, ct);
            return Ok(ApiResponse<IEnumerable<QuestDTO>>.SuccessResponse(activeQuests, "Danh sách nhiệm vụ đang hoạt động"));
        }
    }
}
