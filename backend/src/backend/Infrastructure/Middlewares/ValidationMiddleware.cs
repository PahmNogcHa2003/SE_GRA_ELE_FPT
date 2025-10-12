using Application.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;

namespace Infrastructure.Middlewares
{
    public class ValidationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ValidationMiddleware> _logger;

        public ValidationMiddleware(RequestDelegate next, ILogger<ValidationMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Chặn request trước khi đến Controller
            context.Response.OnStarting(() =>
            {
                // Nếu là lỗi validation -> status code sẽ được đặt trong pipeline dưới
                return Task.CompletedTask;
            });

            // Tiếp tục middleware chain
            await _next(context);

            // Sau khi controller xử lý xong, kiểm tra nếu có lỗi ModelState
            if (context.Items.ContainsKey("ModelValidationErrors"))
            {
                var errors = context.Items["ModelValidationErrors"] as IEnumerable<string>;

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

                var response = ApiResponse<object>.ErrorResponse(
                    message: "Validation Failed",
                    errors: errors
                );

                var json = JsonSerializer.Serialize(response,
                    new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

                await context.Response.WriteAsync(json);
            }
        }
    }
}
