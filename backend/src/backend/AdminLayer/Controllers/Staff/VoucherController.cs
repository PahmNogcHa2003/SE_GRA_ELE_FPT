using Application.Common;
using Application.DTOs.Voucher;
using Application.Interfaces.Staff.Service;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AdminLayer.Controllers.Staff
{
    [Route("api/[controller]")]
    [ApiController]
    public class VoucherController : ControllerBase
    {
        private readonly IVoucherService _voucherService;

        public VoucherController(IVoucherService voucherService)
        {
            _voucherService = voucherService;
        }

        // GET: api/Voucher
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<VoucherDTO>>>> GetVouchers(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? searchQuery = null,
            [FromQuery] string? filterField = null,
            [FromQuery] string? filterValue = null,
            [FromQuery] string? sortOrder = null,
            CancellationToken ct = default)
        {
            try
            {
                var paged = await _voucherService.GetPagedAsync(
                    page, pageSize, searchQuery, filterField, filterValue, sortOrder, ct);

                return Ok(ApiResponse<PagedResult<VoucherDTO>>
                    .SuccessResponse(paged, "Fetched vouchers successfully."));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<PagedResult<VoucherDTO>>
                    .ErrorResponse(ex.Message));
            }
        }

        // GET: api/Voucher/5
        [HttpGet("{id:long}")]
        public async Task<ActionResult<ApiResponse<VoucherDTO>>> GetVoucherById(
            long id,
            CancellationToken ct = default)
        {
            try
            {
                var voucher = await _voucherService.GetAsync(id, ct);
                if (voucher == null)
                    return NotFound(ApiResponse<VoucherDTO>.ErrorResponse("Voucher not found."));

                return Ok(ApiResponse<VoucherDTO>
                    .SuccessResponse(voucher, "Fetched voucher successfully."));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<VoucherDTO>
                    .ErrorResponse(ex.Message));
            }
        }

        // POST: api/Voucher
        [HttpPost]
        public async Task<ActionResult<ApiResponse<VoucherDTO>>> Create(
            [FromBody] createVoucherDto dto,
            CancellationToken ct = default)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ApiResponse<VoucherDTO>.ErrorResponse("Invalid data."));
                }

                var created = await _voucherService.CreateAsync(dto, ct);

                // Nếu DTO của bạn có property khác Id (vd: VoucherId) thì sửa lại ở đây
                return CreatedAtAction(
                    nameof(GetVoucherById),
                    new { id = created.Id },
                    ApiResponse<VoucherDTO>.SuccessResponse(created, "Voucher created successfully."));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<VoucherDTO>.ErrorResponse(ex.Message));
            }
        }

        // PUT: api/Voucher/5
        [HttpPut("{id:long}")]
        public async Task<ActionResult<ApiResponse<VoucherDTO>>> Update(
            long id,
            [FromBody] UpdateVoucherDTO dto,
            CancellationToken ct = default)
        {
            try
            {
                var existing = await _voucherService.GetAsync(id, ct);
                if (existing == null)
                    return NotFound(ApiResponse<VoucherDTO>.ErrorResponse("Voucher not found."));

                await _voucherService.UpdateAsync(id, dto, ct);
                var updated = await _voucherService.GetAsync(id, ct);

                return Ok(ApiResponse<VoucherDTO>
                    .SuccessResponse(updated, "Voucher updated successfully."));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<VoucherDTO>.ErrorResponse(ex.Message));
            }
        }


        // DELETE: api/Voucher/5
        [HttpDelete("{id:long}")]
        public async Task<ActionResult<ApiResponse<object>>> Delete(
            long id,
            CancellationToken ct = default)
        {
            try
            {
                var existing = await _voucherService.GetAsync(id, ct);
                if (existing == null)
                    return NotFound(ApiResponse<object>.ErrorResponse("Voucher not found."));

                await _voucherService.DeleteAsync(id, ct);

                return Ok(ApiResponse<object>.SuccessResponse(null, "Voucher deleted successfully."));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
            }
        }
    }
}
