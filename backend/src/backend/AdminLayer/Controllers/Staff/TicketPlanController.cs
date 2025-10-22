using Application.Common;
using Application.DTOs;
using Application.DTOs.Tickets;
using Application.Interfaces.Staff.Service;
using Application.Services.Staff;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace AdminLayer.Controllers.Staff
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketPlanController : ControllerBase
    {
        private readonly IManageTicketPlanService _manageTicketPlanService;

        public TicketPlanController(IManageTicketPlanService service)
        {
            _manageTicketPlanService = service;
        }

        // GET: api/ManageTicketPlan
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<TicketPlanDTO>>>> GetPlans(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? searchQuery = null,
            [FromQuery] string? filterField = null,
            [FromQuery] string? filterValue = null,
            [FromQuery] string? sortOrder = null,
            CancellationToken ct = default)
        {
            var paged = await _manageTicketPlanService.GetPagedAsync(page, pageSize, searchQuery, filterField, filterValue, sortOrder, ct);
            return Ok(ApiResponse<PagedResult<TicketPlanDTO>>.SuccessResponse(paged, "Fetched ticket plans successfully !"));
        }

        // GET: api/ManageTicketPlan/5
        [HttpGet("{id:long}")]
        public async Task<ActionResult<ApiResponse<TicketPlanDTO>>> GetPlanById(long id, CancellationToken ct = default)
        {
            var plan = await _manageTicketPlanService.GetAsync(id, ct); 
            return Ok(ApiResponse<TicketPlanDTO>.SuccessResponse(plan, "Fetched ticket plan successfully !"));
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<TicketPlanDTO>>> Create([FromBody] CreateTicketPlanDTO dto)
        {
            var created = await _manageTicketPlanService.CreateAsync(dto); 
            return CreatedAtAction(nameof(GetPlanById), new { id = created.Id },
                ApiResponse<TicketPlanDTO>.SuccessResponse(created, "Created new ticket plan successfully !"));
        }

        [HttpPut("{id:long}")]
        public async Task<ActionResult<ApiResponse<TicketPlanDTO>>> Update(long id,[FromBody] UpdateTicketPlanDTO dto,CancellationToken ct = default)
        {
            var updated = await _manageTicketPlanService.GetAsync(id, ct);
            await _manageTicketPlanService.UpdateAsync(id, dto);
            return Ok(ApiResponse<TicketPlanDTO>.SuccessResponse(updated, "Updated ticket plan successfully !"));
        }



        // DELETE: api/ManageTicketPlan/5
        [HttpDelete("{id:long}")]
        public async Task<ActionResult<ApiResponse<object>>> DeletePlan(long id, CancellationToken ct = default)
        {
            await _manageTicketPlanService.DeleteAsync(id, ct);
            return Ok(ApiResponse<object>.SuccessResponse(null, "Ticket plan deleted successfully !"));
        }
    }
}
