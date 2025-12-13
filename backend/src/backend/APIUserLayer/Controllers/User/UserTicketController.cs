using Application.Common;
using Application.DTOs.Tickets;
using Application.Interfaces.User.Service;
using Application.Services.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace APIUserLayer.Controllers.User
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserTicketController : ControllerBase
    {
        private readonly IUserTicketService _svc;

        public UserTicketController(IUserTicketService svc)
        {
            _svc = svc;
        }
        [HttpGet("market")]
        public async Task<IActionResult> Market([FromQuery] string? vehicleType, CancellationToken ct)
        {
            var validTypes = new[] { "Bike", "Ebike"};
            if (!string.IsNullOrEmpty(vehicleType) && !validTypes.Contains(vehicleType, StringComparer.OrdinalIgnoreCase))
            {
                return BadRequest(ApiResponse<string>.ErrorResponse(
                    $"Invalid vehicle type '{vehicleType}'. Allowed values: {string.Join(", ", validTypes)}"
                ));
            }

            var data = await _svc.GetTicketMarketAsync(vehicleType, ct);

            if (data == null || !data.Any())
            {
                return NotFound(ApiResponse<IEnumerable<UserTicketPlanDTO>>.ErrorResponse(
                    "No ticket plans found for the selected vehicle type."
                ));
            }

            return Ok(ApiResponse<IEnumerable<UserTicketPlanDTO>>.SuccessResponse(
                data,
                "Fetched user ticket market successfully."
            ));
        }
        [Authorize]
        [HttpGet("{id:long}")]
        public async Task<ActionResult<ApiResponse<UserTicketDTO>>> GetById(long id, CancellationToken ct = default)
        {
            var userId = User.GetUserIdAsLong();
            var item = await _svc.GetIdByUserIdAsync(userId, id, ct); 

            if (item is null)
                return NotFound(ApiResponse<UserTicketDTO>.ErrorResponse("User ticket not found."));

            return Ok(ApiResponse<UserTicketDTO>.SuccessResponse(item, "Fetched user ticket successfully."));
        }
        [Authorize]
        [HttpGet("active")]
        public async Task<ActionResult<ApiResponse<List<UserTicketDTO>>>> GetMyActiveTickets(
            CancellationToken ct = default)
        {
            var userId = User.GetUserIdAsLong();
            var list = await _svc.GetMyActiveTicketsAsync(userId, ct);
            return Ok(ApiResponse<List<UserTicketDTO>>.SuccessResponse(list, "Fetched active tickets successfully."));
        }
        [Authorize]
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
        [Authorize]
        [HttpPost("preview")]
        public async Task<ActionResult<ApiResponse<PreviewTicketPriceDTO>>> Preview([FromBody] PreviewTicketRequestDTO request, CancellationToken ct = default)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<PreviewTicketPriceDTO>.ErrorResponse("Invalid data."));

            try
            {
                var userId = User.GetUserIdAsLong();
                var result = await _svc.PreviewTicketPriceAsync(userId, request, ct);
                return Ok(ApiResponse<PreviewTicketPriceDTO>.SuccessResponse(result, "Preview ticket price successfully."));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<PreviewTicketPriceDTO>.ErrorResponse(ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<PreviewTicketPriceDTO>.ErrorResponse(ex.Message));
            }
            catch (Exception)
            {
                return StatusCode(500, ApiResponse<PreviewTicketPriceDTO>.ErrorResponse("Internal server error."));
            }
        }
    }
}
