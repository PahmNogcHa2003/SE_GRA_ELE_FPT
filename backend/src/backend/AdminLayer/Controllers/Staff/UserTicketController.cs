using Application.Common;
using Application.DTOs.Tickets;
using Application.Interfaces.Staff.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AdminLayer.Controllers.Staff
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserTicketController : ControllerBase
    {
        private readonly IManageUserTicketService _service;
        public UserTicketController(IManageUserTicketService service)
        {
            _service = service;
        }
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<ManageUserTicketDTO>>>> GetPaged(
           [FromQuery] int page = 1,
           [FromQuery] int pageSize = 10,
           [FromQuery] string? searchQuery = null,
           [FromQuery] string? filterField = null,
           [FromQuery] string? filterValue = null,
           [FromQuery] string? sortOrder = null,
           CancellationToken ct = default)
        {
            var result = await _service.GetPagedAsync(page, pageSize, searchQuery, filterField, filterValue, sortOrder, ct);
            return Ok(ApiResponse<PagedResult<ManageUserTicketDTO>>.SuccessResponse(result, "Fetched user tickets successfully."));
        }
        [HttpGet("{id:long}")]
        public async Task<ActionResult<ApiResponse<ManageUserTicketDTO>>> GetById(long id, CancellationToken ct = default)
        {
            var dto = await _service.GetAsync(id, ct);
            if (dto == null)
                return NotFound(ApiResponse<ManageUserTicketDTO>.ErrorResponse("User ticket not found."));

            return Ok(ApiResponse<ManageUserTicketDTO>.SuccessResponse(dto, "Fetched user ticket successfully."));
        }
        public class VoidRequest
        {
            public string Reason { get; set; } = string.Empty;
        }

        [HttpPost("{id:long}/void")]
        public async Task<ActionResult<ApiResponse<ManageUserTicketDTO>>> VoidTicket(long id, [FromBody] VoidRequest req, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(req.Reason))
                return BadRequest(ApiResponse<ManageUserTicketDTO>.ErrorResponse("Reason is required."));

            try
            {
                var result = await _service.VoidTicketAsync(id, req.Reason, ct);
                return Ok(ApiResponse<ManageUserTicketDTO>.SuccessResponse(result, "Ticket voided successfully."));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<ManageUserTicketDTO>.ErrorResponse(ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<ManageUserTicketDTO>.ErrorResponse(ex.Message));
            }
            catch (Exception)
            {
                return StatusCode(500, ApiResponse<ManageUserTicketDTO>.ErrorResponse("Internal server error."));
            }
        }
    }
}
