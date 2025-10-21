using Application.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace APIUserLayer.Controllers.Base
{
    [ApiController]
    public abstract class ApiBaseController : ControllerBase
    {
        protected IActionResult Success<T>(T? data = default, string message = "Success")
        {
            var result = ApiResponse<T>.SuccessResponse(data, message);
            return StatusCode(200, result);
        }

        protected IActionResult Error(string message = "Error", IEnumerable<string>? errors = null, int statusCode = 400)
        {
            var result = ApiResponse<object>.ErrorResponse(message, errors);
            return StatusCode(statusCode, result);
        }
    }
}
