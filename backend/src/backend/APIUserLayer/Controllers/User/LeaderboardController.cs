using Application.Common;
using Application.DTOs.RentalHistory;
using Application.Interfaces.User.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace APIUserLayer.Controllers.User
{
    [Route("api/[controller]")]
    [ApiController]

    public class LeaderboardController : ControllerBase
    {
        private readonly IUserLifetimeStatsService _userLifetimeStatsService;
        public LeaderboardController(IUserLifetimeStatsService userLifetimeStatsService)
        {
            _userLifetimeStatsService = userLifetimeStatsService;
        }
        /// <summary>
        /// Lấy bảng xếp hạng người dùng theo quãng đường / thời gian đi xe trong các khoảng thời gian khác nhau
        /// </summary>
        /// <param name="period"></param>
        /// <param name="topN"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LeaderboardEntryDTO>>> GetLeaderboard([FromQuery] string period = "lifetime", [FromQuery] int topN = 10, CancellationToken ct = default)
        {
            var leaderboard = await _userLifetimeStatsService.GetLeaderboardAsync(period, topN, ct);
            return Ok(ApiResponse<IEnumerable<LeaderboardEntryDTO>>.SuccessResponse(leaderboard, $"Bảng xếp hạng theo period = {period}"));
        }
    }
}
