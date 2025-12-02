using Application.Common;
using Application.DTOs;
using Application.DTOs.WalletTransaction;
using Application.Interfaces.User.Service;
using Application.Services.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace APIUserLayer.Controllers
{
    [Route("api/wallet/transactions")]
    [ApiController]
    public class WalletTransactionsController : ControllerBase
    {
        private readonly IWalletTransactionService _transactionService;

        public WalletTransactionsController(IWalletTransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpGet]
        public async Task<IActionResult> GetTransactionsByUserId(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? sortOrder = "createdAt_desc",
            CancellationToken ct = default)
        {
            var userId = User.GetUserIdAsLong();
            var pagedResult = await _transactionService.GetTransactionsByUserIdAsync(userId, page, pageSize, sortOrder, ct);
            return Ok(ApiResponse<PagedResult<WalletTransactionDTO>>.SuccessResponse(pagedResult));
        }
    }
}
