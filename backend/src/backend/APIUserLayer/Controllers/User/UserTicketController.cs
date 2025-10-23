using Application.Common;
using Application.DTOs.Tickets;
using Application.Interfaces.User.Service;
using Application.Services.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace APIUserLayer.Controllers.User
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserTicketController : ControllerBase
    {
        private readonly IUserTicketService _svc;

        public UserTicketController(IUserTicketService svc)
        {
            _svc = svc;
        }

        // GET: api/UserTicket/123
        [HttpGet("{id:long}")]
        public async Task<ActionResult<ApiResponse<UserTicketDTO>>> GetById(
            long id,
            CancellationToken ct = default)
        {
            var item = await _svc.GetAsync(id, ct);
            if (item is null)
                return NotFound(ApiResponse<UserTicketDTO>.ErrorResponse("User ticket not found."));

            return Ok(ApiResponse<UserTicketDTO>.SuccessResponse(item, "Fetched user ticket successfully."));
        }

        // GET: api/UserTicket/users/45/active
        [HttpGet("users/{userId:long}/active")]
        public async Task<ActionResult<ApiResponse<List<UserTicketDTO>>>> GetMyActiveTickets(
            long userId,
            CancellationToken ct = default)
        {
            var list = await _svc.GetMyActiveTicketsAsync(userId, ct);
            return Ok(ApiResponse<List<UserTicketDTO>>.SuccessResponse(list, "Fetched active tickets successfully."));
        }

        // POST: api/UserTicket/purchase

        [HttpPost("purchase")]
        public async Task<ActionResult<ApiResponse<UserTicketDTO>>> Purchase(
            [FromBody] PurchaseTicketRequestDTO request,
            CancellationToken ct = default)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<UserTicketDTO>.ErrorResponse("Invalid data."));

            try
            {
                var userId = User.GetUserIdAsLong(); // 👈 lấy từ JWT
                var created = await _svc.PurchaseTicketAsync(userId, request, ct); // 👈 truyền xuống service
                return CreatedAtAction(nameof(GetById), new { id = created.Id },
                    ApiResponse<UserTicketDTO>.SuccessResponse(created, "Purchased ticket successfully."));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<UserTicketDTO>.ErrorResponse(ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<UserTicketDTO>.ErrorResponse(ex.Message));
            }
            catch (Exception)
            {
                return StatusCode(500, ApiResponse<UserTicketDTO>.ErrorResponse("Internal server error."));
            }
        }
    }
}
