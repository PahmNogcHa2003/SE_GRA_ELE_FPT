using Application.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;

namespace Infrastructure.Middlewares
{
    public class ResponseMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ResponseMiddleware> _logger;

        public ResponseMiddleware(RequestDelegate next, ILogger<ResponseMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception");

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                var response = ApiResponse<object>.ErrorResponse(
                    message: "Internal Server Error",
                    errors: new[] { ex.Message }
                );

                var json = JsonSerializer.Serialize(response,
                    new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

                await context.Response.WriteAsync(json);
            }
        }
    }
}
