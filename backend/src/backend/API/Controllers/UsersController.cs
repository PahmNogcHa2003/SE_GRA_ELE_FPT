using Application.DTOs;
using Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserService _userService;
        public UsersController(UserService userService)
        {
            _userService = userService;
        }

        [HttpGet("{id}")]   
        public async Task<IActionResult> GetUserById(long id)
        {
            var dto = await _userService.GetAsync(id);
            if(dto == null) return NotFound();
            return Ok(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Create(UserDTO dto)
        {
            var created = await _userService.CreateAsync(dto);
            return Ok();
        }
    }
}
