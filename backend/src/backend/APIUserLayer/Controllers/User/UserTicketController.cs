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
        // MARKET: hiển thị các gói & giá theo loại xe
        // GET /api/user-tickets/market?vehicleType=Bike
        // hoặc yêu cầu login tùy bạn
        [HttpGet("market")]
        public async Task<IActionResult> Market([FromQuery] string? vehicleType, CancellationToken ct)
        {
            var data = await _svc.GetTicketMarketAsync(vehicleType, ct);
            return Ok(data);
        }
        // GET: api/UserTicket/123
        [HttpGet("{id:long}")]
        public async Task<ActionResult<ApiResponse<UserTicketDTO>>> GetById(long id, CancellationToken ct = default)
        {
            var userId = User.GetUserIdAsLong();
            var item = await _svc.GetIdByUserIdAsync(userId, id, ct); // đảm bảo cùng chủ sở hữu

            if (item is null)
                return NotFound(ApiResponse<UserTicketDTO>.ErrorResponse("User ticket not found."));

            return Ok(ApiResponse<UserTicketDTO>.SuccessResponse(item, "Fetched user ticket successfully."));
        }

        // GET: api/UserTicket/users/45/active
        [HttpGet("active")]
        public async Task<ActionResult<ApiResponse<List<UserTicketDTO>>>> GetMyActiveTickets(
            CancellationToken ct = default)
        {
            var userId = User.GetUserIdAsLong();
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
                var userId = User.GetUserIdAsLong(); 
                var created = await _svc.PurchaseTicketAsync(userId, request, ct);
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
