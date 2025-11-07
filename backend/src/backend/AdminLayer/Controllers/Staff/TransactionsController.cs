using Application.Common;
using Application.DTOs.Transactions;
using Application.Interfaces.Staff.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AdminLayer.Controllers.Staff
{
    //[Authorize(Roles = "Staff")]
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionService _transactionService;

        public TransactionsController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<TransactionsDTO>>>> GetTransactionAsync(
            [FromQuery] TransactionQueryParams query,
            CancellationToken ct = default)
        {
            var data = await _transactionService.GetTransactionAsync(query, ct);
            return Ok(ApiResponse<PagedResult<TransactionsDTO>>.SuccessResponse(data, "Fetched transactions"));
        }
    }
}

