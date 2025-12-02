using Application.Common;
using Application.DTOs.Tickets;
using Application.Interfaces.Staff.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AdminLayer.Controllers.Staff
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketPlanPriceController : ControllerBase
    {
        private readonly ITicketPlanPriceService _svc;

        public TicketPlanPriceController(ITicketPlanPriceService svc)
        {
            _svc = svc;
        }
        
        // GET: api/TicketPlanPrice
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<TicketPlanPriceDTO>>>> GetPrices(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? searchQuery = null,      
            [FromQuery] string? filterField = null,      
            [FromQuery] string? filterValue = null,      
            [FromQuery] string? sortOrder = null,        
            CancellationToken ct = default)
        {
            var paged = await _svc.GetPagedAsync(page, pageSize, searchQuery, filterField, filterValue, sortOrder, ct);
            return Ok(ApiResponse<PagedResult<TicketPlanPriceDTO>>.SuccessResponse(paged, "Fetched ticket plan prices successfully!"));
        }

        // GET: api/TicketPlanPrice/5
        [HttpGet("{id:long}")]
        public async Task<ActionResult<ApiResponse<TicketPlanPriceDTO>>> GetById(long id, CancellationToken ct = default)
        {
            var item = await _svc.GetAsync(id, ct);
            if (item is null)
                return NotFound(ApiResponse<TicketPlanPriceDTO>.ErrorResponse("Ticket plan price not found!"));

            return Ok(ApiResponse<TicketPlanPriceDTO>.SuccessResponse(item, "Fetched ticket plan price successfully!"));
        }

        // POST: api/TicketPlanPrice
        [HttpPost]
        public async Task<ActionResult<ApiResponse<TicketPlanPriceDTO>>> Create(
            [FromBody] CreateTicketPlanPriceDTO dto,
            CancellationToken ct = default)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<TicketPlanPriceDTO>.ErrorResponse("Invalid data."));

            var created = await _svc.CreateAsync(dto, ct); 
            return CreatedAtAction(nameof(GetById), new { id = created.Id },
                ApiResponse<TicketPlanPriceDTO>.SuccessResponse(created, "Created ticket plan price successfully!"));
        }

        // PUT: api/TicketPlanPrice/5
        [HttpPut("{id:long}")]
        public async Task<ActionResult<ApiResponse<TicketPlanPriceDTO>>> Update(
            long id,
            [FromBody] UpdateTicketPlanPriceDTO dto,
            CancellationToken ct = default)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<TicketPlanPriceDTO>.ErrorResponse("Invalid data."));
            await _svc.UpdateAsync(id, dto, ct);

            // lấy lại dữ liệu sau cập nhật
            var updated = await _svc.GetAsync(id, ct);
            if (updated is null)
                return NotFound(ApiResponse<TicketPlanPriceDTO>.ErrorResponse("Ticket plan price not found after update."));

            return Ok(ApiResponse<TicketPlanPriceDTO>.SuccessResponse(updated, "Updated ticket plan price successfully!"));
        }

        // DELETE: api/TicketPlanPrice/5
        [HttpDelete("{id:long}")]
        public async Task<ActionResult<ApiResponse<object>>> Delete(long id, CancellationToken ct = default)
        {
            var exist = await _svc.GetAsync(id, ct);
            if (exist is null)
                return NotFound(ApiResponse<object>.ErrorResponse("Ticket plan price not found!"));

            await _svc.DeleteAsync(id, ct);
            return Ok(ApiResponse<object>.SuccessResponse(null, "Ticket plan price deleted successfully!"));
        }
    }
}

