using Application.Common;
using Application.DTOs;
using Application.Interfaces.User.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace APIUserLayer.Controllers.User
{
    [Route("api/wallets")]
    [ApiController]
    public class WalletsController : ControllerBase
    {
        private readonly IUserWalletService _walletService;

        public WalletsController(IUserWalletService walletService)
        {
            _walletService = walletService;
        }

        // GET: api/wallets/user/{userId}
        [Authorize]
        [HttpGet("user/{userId:long}")] 
        public async Task<IActionResult> GetWalletByUserId(long userId, CancellationToken ct)
        {
            var walletDto = await _walletService.GetByUserIdAsync(userId, ct);

            if (walletDto == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse($"Wallet for user with id {userId} not found."));
            }
            return Ok(ApiResponse<WalletDTO>.SuccessResponse(walletDto));
        }
    }
}
