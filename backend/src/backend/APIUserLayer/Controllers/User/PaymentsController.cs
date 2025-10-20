using Application.DTOs.Payments;
using Application.Interfaces.User.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;

namespace APIUserLayer.Controllers.User
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentsController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }
        [HttpPost("vnpay/create-url")]
        public async Task<IActionResult> CreateVnPayPaymentUrl(
        [FromBody] CreatePaymentRequestDTO request,
        CancellationToken cancellationToken)
        {
            var result = await _paymentService.CreateVnPayPaymentUrlAsync(request, HttpContext, cancellationToken);
            return Ok(result);
        }

        // ReturnUrl: GET
        [HttpGet("vnpay-return")]
        public async Task<IActionResult> VnPayReturn(CancellationToken ct)
        {
            // Log inbound
            Console.WriteLine($"[RETURN] {Request.Method} {Request.Path}{Request.QueryString}");
            foreach (var h in Request.Headers) Console.WriteLine($"[H] {h.Key}: {h.Value}");

            // Truyền nguyên Query (giữ nguyên method cũ)
            var result = await _paymentService.ProcessVnPayCallbackAsync(Request.Query, ct);
            // Tuỳ bạn: redirect về FE hoặc trả result
            return Ok(result);
        }

      
    }
}
