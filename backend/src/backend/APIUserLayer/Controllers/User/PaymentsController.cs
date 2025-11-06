// APIUserLayer/Controllers/User/PaymentsController.cs
using Microsoft.AspNetCore.Authorization;
using Application.DTOs.Payments;
using Application.Interfaces.User.Service;
using Azure.Core;
using Microsoft.AspNetCore.Mvc;
// using ... ClaimsPrincipalExtensions

[Route("api/[controller]")]
[ApiController]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentService _paymentService;

    public PaymentsController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    // Người dùng tạo URL thanh toán → phải đăng nhập
    [Authorize]
    [HttpPost("vnpay/create-url")]
    public async Task<IActionResult> CreateVnPayPaymentUrl(
        [FromBody] CreatePaymentRequestDTO request,
        CancellationToken cancellationToken)
    {
        // Không dùng request.UserId nữa — lấy từ token
        var result = await _paymentService.CreateVnPayPaymentUrlAsync(request, HttpContext, cancellationToken);
        return Ok(result);
    }

    // VNPay return/callback: thường KHÔNG cần [Authorize]
    [HttpGet("vnpay-return")]
    public async Task<IActionResult> VnPayReturn(CancellationToken ct)
    {
        var result = await _paymentService.ProcessVnPayCallbackAsync(Request.Query, ct);
        return Ok(result);
    }
}
