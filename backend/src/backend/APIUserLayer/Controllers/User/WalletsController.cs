using Application.Common;
using Application.DTOs;
using Application.DTOs.Wallet;
using Application.DTOs.WalletTransaction;
using Application.Interfaces.User.Service;
using Application.Services.Identity;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace APIUserLayer.Controllers.User
{
    [Route("api/wallets")]
    [ApiController]
    public class WalletsController : ControllerBase
    {
        private readonly IUserWalletService _userWalletService;
        private readonly IWalletService _walletService;
        public WalletsController(IUserWalletService userWalletService, IWalletService walletService)
        {
            _userWalletService = userWalletService;
            _walletService = walletService;
        }

        // GET: api/wallets/user/{userId}
        [Authorize]
        [HttpGet] 
        public async Task<IActionResult> GetWalletByUserId(CancellationToken ct)
        {
            var userId = User.GetUserIdAsLong();
            var walletDto = await _userWalletService.GetByUserIdAsync(userId, ct);

            if (walletDto == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse($"Wallet for user with id {userId} not found."));
            }
            return Ok(ApiResponse<WalletDTO>.SuccessResponse(walletDto));
        }
        [HttpPost("convert-promo")]
        public async Task<IActionResult> ConvertPromo([FromBody] ConvertPromoRequest req, CancellationToken ct)
        {
            if (req == null)
                return BadRequest(ApiResponse<WalletTransaction>.ErrorResponse("Request body is required."));

            if (req.Amount <= 0)
                return BadRequest(ApiResponse<WalletTransaction>.ErrorResponse("Số tiền phải lớn hơn hoặc bằng 0."));

            var userId = User.GetUserIdAsLong();

            try
            {
                var txn = await _walletService.ConvertPromoToBalanceAsync(userId, req.Amount, ct);
                return Ok(ApiResponse<WalletTransaction>.SuccessResponse(txn));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<WalletTransaction>.ErrorResponse(ex.Message));
            }
        }
        public class ConvertPromoRequest
        {
            public decimal Amount { get; set; }
        }
        [HttpPost("pay-all-debt")]
        public async Task<IActionResult> PayAllDebt(CancellationToken ct)
        {
            try
            {
                var userId = User.GetUserIdAsLong();
                var result = await _walletService.PayAllDebtFromBalanceAsync(userId, ct);

                return Ok(ApiResponse<PayDebtResultDTO>.SuccessResponse(
                    result,
                    $"Đã thanh toán {result.PaidAmount:N0}đ. Nợ còn lại: {result.RemainingDebt:N0}đ."
                ));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<PayDebtResultDTO>.ErrorResponse(ex.Message));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<PayDebtResultDTO>.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                // Log thêm ex nếu cần
                return StatusCode(500,
                    ApiResponse<PayDebtResultDTO>.ErrorResponse("Có lỗi hệ thống khi thanh toán nợ."));
            }
        }


    }
}
