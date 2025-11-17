using Application.DTOs.Auth;
using Application.Interfaces.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq; // For selecting error messages from ModelState
using System.Threading.Tasks;
using Application.Common;
using Microsoft.AspNetCore.Authorization; // Assuming your ApiResponse is here

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
        [AllowAnonymous]
        [ProducesResponseType(typeof(AuthResponseDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(AuthResponseDTO), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AuthResponseDTO>> Register([FromBody] RegisterDTO model, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToArray();

                return BadRequest(new AuthResponseDTO
                {
                    IsSuccess = false,
                    Message = string.Join(" | ", errors)
                });
            }

            var result = await _authService.RegisterAsync(model, ct);

            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }

            result.Message = result.Message ?? "User registration successful.";
            return StatusCode(StatusCodes.Status201Created, result);
        }


        /// <summary>
        /// Logs in a user and returns a JWT token.
        /// </summary>
        /// <param name="model">The login credentials.</param>
        /// <returns>HTTP 200 on success, HTTP 401 on authentication failure.</returns>
        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(AuthResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(AuthResponseDTO), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(AuthResponseDTO), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AuthResponseDTO>> Login([FromBody] LoginDTO model, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToArray();

                return BadRequest(new AuthResponseDTO
                {
                    IsSuccess = false,
                    Message = string.Join(" | ", errors)
                });
            }

            var result = await _authService.LoginAsync(model, ct);

            if (!result.IsSuccess || string.IsNullOrWhiteSpace(result.Token))
            {
                return Unauthorized(new AuthResponseDTO
                {
                    IsSuccess = false,
                    Message = "Invalid email or password."
                });
            }
            return Ok(result);
        }


        [HttpGet("me")]
        [Authorize]
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
        [Authorize]
        [HttpPost("change-password")]
        [ProducesResponseType(typeof(AuthResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(AuthResponseDTO), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(AuthResponseDTO), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<AuthResponseDTO>> ChangePassword([FromBody] ChangePasswordDTO dto, CancellationToken ct)
        {
            if (!User.Identity?.IsAuthenticated ?? true)
            {
                return Unauthorized(new AuthResponseDTO
                {
                    IsSuccess = false,
                    Message = "Unauthorized."
                });
            }

            var result = await _authService.ChangePasswordAsync(User, dto, ct);
            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

    }
}