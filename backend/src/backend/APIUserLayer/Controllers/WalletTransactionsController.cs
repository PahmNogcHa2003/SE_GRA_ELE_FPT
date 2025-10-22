using Application.Common;
using Application.DTOs;
using Application.Interfaces.User.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace APIUserLayer.Controllers
{
    [Route("api/wallet-transactions")]
    [ApiController]
    public class WalletTransactionsController : ControllerBase
    {
        private readonly IWalletTransactionService _transactionService;

        public WalletTransactionsController(IWalletTransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        // GET: api/wallet-transactions/user/{userId}
        [HttpGet("user/{userId:long}")]
        public async Task<IActionResult> GetTransactionsByUserId(
            long userId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? sortOrder = "createdAt_desc",
            CancellationToken ct = default)
        {
            var pagedResult = await _transactionService.GetTransactionsByUserIdAsync(userId, page, pageSize, sortOrder, ct);
            return Ok(ApiResponse<PagedResult<WalletTransactionDTO>>.SuccessResponse(pagedResult));
        }
    }
}
