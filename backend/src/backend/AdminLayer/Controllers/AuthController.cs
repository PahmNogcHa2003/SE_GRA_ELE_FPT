using Application.DTOs.Auth;
using Application.Interfaces.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq; // For selecting error messages from ModelState
using System.Threading.Tasks;
using Application.Common; // Assuming your ApiResponse is here

namespace Admin.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Registers a new user.
        /// </summary>
        /// <param name="model">The registration data.</param>
        /// <returns>HTTP 201 on success, HTTP 400 on failure.</returns>
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<AuthResponseDTO>>> Register([FromBody] RegisterDTO model)
        {
            // 1. Model State Validation (Ensure the DTO is valid)
            if (!ModelState.IsValid)
            {
                // Return 400 Bad Request with validation errors
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToArray();
                return BadRequest(ApiResponse<AuthResponseDTO>.ErrorResponse("Validation Failed", errors));
            }

            // 2. Call the Application Service
            var result = await _authService.RegisterAsync(model);

            if (!result.IsSuccess)
            {
                // 400 Bad Request for business logic failures (e.g., Email already exists)
                var errorResponse = ApiResponse<AuthResponseDTO>.ErrorResponse("Registration Failed", new[] { result.Message! });
                return BadRequest(errorResponse);
            }

            // 3. Success: Return 201 Created (since a new resource/user was created)
            // Note: We create a new DTO to clean up the response object if needed
            var data = new AuthResponseDTO { /* Map necessary data, typically no token here */ };
            var successResponse = ApiResponse<AuthResponseDTO>.SuccessResponse(data, "User registration successful.");

            // Use Created to follow REST principles for resource creation
            return StatusCode(StatusCodes.Status201Created, successResponse);
        }

        /// <summary>
        /// Logs in a user and returns a JWT token.
        /// </summary>
        /// <param name="model">The login credentials.</param>
        /// <returns>HTTP 200 on success, HTTP 401 on authentication failure.</returns>
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ApiResponse<AuthResponseDTO>>> Login([FromBody] LoginDTO model)
        {
            // 1. Model State Validation
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToArray();
                return BadRequest(ApiResponse<AuthResponseDTO>.ErrorResponse("Validation Failed", errors));
            }

            // 2. Call the Application Service
            var result = await _authService.LoginAsync(model);

            if (!result.IsSuccess)
            {
                // 401 Unauthorized for invalid credentials (standard for failed authentication)
                var errorResponse = ApiResponse<AuthResponseDTO>.ErrorResponse("Authentication Failed", new[] { "Invalid email or password." });
                return Unauthorized(errorResponse);
            }

            // 3. Success: Return 200 OK with the token data
            var data = new AuthResponseDTO { Token = result.Token! };
            var successResponse = ApiResponse<AuthResponseDTO>.SuccessResponse(data, "Login successful.");

            return Ok(successResponse);
        }
        [HttpGet("me")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<MeDTO>>> Me(CancellationToken ct)
        {
            if (User?.Identity?.IsAuthenticated != true)
            {
                return Unauthorized(ApiResponse<MeDTO>.ErrorResponse("Unauthorized"));
            }

            var me = await _authService.GetMeAsync(User, ct);
            if (me == null)
            {
                return NotFound(ApiResponse<MeDTO>.ErrorResponse("User not found"));
            }

            return Ok(ApiResponse<MeDTO>.SuccessResponse(me, "OK"));
        }
    }
}