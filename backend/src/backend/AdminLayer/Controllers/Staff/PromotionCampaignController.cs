using Application.Common;
using Application.DTOs.Promotion;
using Application.Interfaces.Staff.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AdminLayer.Controllers.Staff
{
    [Route("api/[controller]")]
    [ApiController]
    public class PromotionCampaignController : ControllerBase
    {
        private readonly IPromotionCampaignService _promotionCampaignService;
        public PromotionCampaignController(IPromotionCampaignService promotionCampaignService)
        {
            _promotionCampaignService = promotionCampaignService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<PromotionDTO>>>> GetPaged(
            int page = 1,
            int pageSize = 10,
            string? searchQuery = null,
            string? filterField = null,
            string? filterValue = null,
            string? sortOrder = null,
            CancellationToken ct = default)
        {
            var promotions = await _promotionCampaignService.GetPagedAsync(page, pageSize, searchQuery, filterField, filterValue, sortOrder, ct);
            return Ok(ApiResponse<PagedResult<PromotionDTO>>.SuccessResponse(promotions, "Lấy danh sách chiến dịch khuyến mãi thành công."));
        }

        [HttpPost("create")]
        public async Task<ActionResult<ApiResponse<PromotionDTO>>> Create([FromBody] PromotionCreateDTO dto, CancellationToken ct)
        {
            var created = await _promotionCampaignService.CreateAsync(dto, ct);
            return Ok(ApiResponse<PromotionDTO>.SuccessResponse(created, "Tạo chiến dịch khuyến mãi thành công."));
        }
        [HttpPut("{id}/status")]
        public async Task<ActionResult<ApiResponse<PromotionDTO>>> UpdateStatus(long id, [FromQuery] string status, CancellationToken ct)
        {
            var updated = await _promotionCampaignService.UpdateStatusAsync(id, status, ct);
            return Ok(ApiResponse<PromotionDTO>.SuccessResponse(updated, "Cập nhật trạng thái thành công"));
        }
    }
}
