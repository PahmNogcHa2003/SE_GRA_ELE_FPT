using Application.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace APIUserLayer.Controllers.Base
{
    [ApiController]
    [Route("api/user/[controller]")]
    [Authorize(Policy = "UserOnly")]
    public abstract class UserBaseController : ControllerBase
    {
        /// <summary>
        /// Creates a standardized successful API response with a 200 OK status.
        /// </summary>
        /// <param name="data">The data to be returned in the response.</param>
        /// <param name="message">A descriptive message for the response.</param>
        /// <returns>An IActionResult containing the successful response.</returns>
        protected IActionResult Success<T>(T? data = default, string message = "Success")
        {
            var result = ApiResponse<T>.SuccessResponse(data, message);
            return Ok(result);
        }

        /// <summary>
        /// Creates a standardized error API response with a specified status code.
        /// </summary>
        /// <param name="message">The main error message.</param>
        /// <param name="errors">A list of specific error details.</param>
        /// <param name="statusCode">The HTTP status code for the error (default is 400 Bad Request).</param>
        /// <returns>An IActionResult containing the error response.</returns>
        protected IActionResult Error(string message = "Error", IEnumerable<string>? errors = null, int statusCode = 400)
        {
            var result = ApiResponse<object>.ErrorResponse(message, errors);
            return StatusCode(statusCode, result);
        }
    }
}

