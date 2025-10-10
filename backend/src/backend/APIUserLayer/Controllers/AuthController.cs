using Application.DTOs.Auth;
using Application.Interfaces.Identity;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
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

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO model)
        {
            var result = await _authService.RegisterAsync(model);
            if (!result.IsSuccess)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO model)
        {
            var result = await _authService.LoginAsync(model);
            if (!result.IsSuccess)
                return BadRequest(result);
            return Ok(result);
        }
    }
}
