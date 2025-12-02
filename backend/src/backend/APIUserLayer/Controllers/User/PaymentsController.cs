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
    [Authorize]
    [HttpPost("vnpay/create-url")]
    public async Task<IActionResult> CreateVnPayPaymentUrl(
        [FromBody] CreatePaymentRequestDTO request,
        CancellationToken cancellationToken)
    {
        var result = await _paymentService.CreateVnPayPaymentUrlAsync(request, HttpContext, cancellationToken);
        return Ok(result);
    }
    [HttpGet("vnpay-return")]
    public async Task<IActionResult> VnPayReturn(CancellationToken ct)
    {
        var result = await _paymentService.ProcessVnPayCallbackAsync(Request.Query, ct);
        return Ok(result);
    }
}
