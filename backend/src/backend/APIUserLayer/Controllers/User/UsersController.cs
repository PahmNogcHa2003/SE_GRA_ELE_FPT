using Application.DTOs;
using Application.Interfaces.User.Service;
using Application.Services.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace APIUserLayer.Controllers.User
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {

      
            private readonly IUserService _userService;

            public UsersController(IUserService userService)
            {
                _userService = userService;
            }




        [HttpGet]
        public async Task<IActionResult> GetAllUsersByStatus()
        {
            var users = await _userService.GetPagedAsync(1,20);
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(long id)
        {
            var dto = await _userService.GetAsync(id);
            if (dto == null) return NotFound();
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
