using APIUserLayer.Controllers.Base;
using Application.Common;
using Application.DTOs.Contact;
using Application.Interfaces.Staff.Service;
using Application.Interfaces.User.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace APIUserLayer.Controllers.User
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactsController : UserBaseController
    {
        private readonly IContactService _contactService;

        public ContactsController(IContactService contactService)
        {
            _contactService = contactService;
        }

        /// <summary>
        /// Guest gửi liên hệ (contact form)
        /// </summary>
        [HttpPost]
        [AllowAnonymous] // vì guest có thể gửi mà không cần đăng nhập
        [HttpPost]
        public async Task<ActionResult<ApiResponse<CreateContactDTO>>> CreateContact([FromBody] CreateContactDTO createDto)
        {
            var created = await _contactService.CreateAsync(createDto);
            var response = ApiResponse<CreateContactDTO>.SuccessResponse(created, "Contact created successfully");
            return Ok(response);
        }


        /// <summary>
        /// Lấy chi tiết contact theo ID
        /// </summary>
        [HttpGet("{id:long}")]
        public async Task<ActionResult<ApiResponse<CreateContactDTO>>> GetContactById(long id)
        {
            var contact = await _contactService.GetAsync(id);
            return Ok(ApiResponse<CreateContactDTO>.SuccessResponse(contact, "Fetched contact successfully"));
        }
    }
}
