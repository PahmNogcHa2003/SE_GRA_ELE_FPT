using APIUserLayer.Controllers.Base;
using Application.Common;
using Application.DTOs;
using Application.DTOs.Contact;
using Application.Interfaces.Staff.Service;
using Application.Services.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using static QRCoder.PayloadGenerator;

namespace AdminLayer.Controllers.Staff
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ManageContactsController : ApiBaseController
    {
        private readonly IManageContactService _contactsService;

        public ManageContactsController(IManageContactService contactsService)
        {
            _contactsService = contactsService;
        }

        // ✅ GET: api/managecontacts
        [HttpGet]
        public async Task<IActionResult> GetContacts(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null,
            [FromQuery] string? filterField = null,
            [FromQuery] string? filterValue = null,
            [FromQuery] string? sortOrder = null)
        {
            var pagedContacts = await _contactsService.GetPagedAsync(page, pageSize, search, filterField, filterValue, sortOrder);
            return Success(pagedContacts, "Fetched contacts successfully");
        }

        // ✅ GET: api/managecontacts/{id}
        [HttpGet("{id:long}")]
        public async Task<IActionResult> GetContactById(long id)
        {
            var contact = await _contactsService.GetAsync(id);
            return Success(contact, "Fetched contact successfully");
        }

        // ✅ POST: api/managecontacts
        [HttpPost]
        public async Task<IActionResult> CreateContact([FromBody] ManageContactDTO createDto)
        {
            var createdContact = await _contactsService.CreateAsync(createDto);
            return CreatedAtAction(nameof(GetContactById), new { id = createdContact.Id },
                ApiResponse<ManageContactDTO>.SuccessResponse(createdContact, "Contact created successfully"));
        }

        // ✅ PUT: api/managecontacts/{id}
        [HttpPut("{id:long}")]
        public async Task<IActionResult> UpdateContact(long id, [FromBody] ManageContactDTO updateDto)
        {
            if (updateDto.Id != 0 && id != updateDto.Id)
                return Error("Route ID and body ID do not match", new[] { "Invalid ID parameter" }, 400);

            await _contactsService.UpdateAsync(id, updateDto);
            return Success<object>(null, "Contact updated successfully");
        }

        // ✅ DELETE: api/managecontacts/{id}
        [HttpDelete("{id:long}")]
        public async Task<IActionResult> DeleteContact(long id)
        {
            await _contactsService.DeleteAsync(id);
            return Success<object>(null, "Contact deleted successfully");
        }
    }
}
