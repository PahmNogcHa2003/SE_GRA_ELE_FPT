// Infrastructure/Middlewares/ExceptionHandlingMiddleware.cs
using Application.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic; // Cần cho KeyNotFoundException
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace Infrastructure.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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
                _logger.LogError(ex, "An unhandled exception has occurred.");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            var response = ApiResponse<object>.ErrorResponse();

            switch (exception)
            {
                // Lỗi không tìm thấy tài nguyên (404)
                case KeyNotFoundException:
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    response.Message = "The requested resource was not found.";
                    response.Errors = new[] { exception.Message };
                    break;

                // Bạn có thể thêm các loại Exception nghiệp vụ khác ở đây
                // case MyCustomValidationException ex:
                //     context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                //     response.Message = "Validation failed.";
                //     response.Errors = ex.Errors;
                //     break;

                // Mặc định là lỗi server (500)
                default:
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    response.Message = "An internal server error has occurred.";
                    response.Errors = new[] { exception.Message }; // Chỉ trả về message ở môi trường Dev
                    break;
            }

            var json = JsonSerializer.Serialize(response, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            await context.Response.WriteAsync(json);
        }
    }
}