using Application.Common;
using Application.DTOs.Quest;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AdminLayer.Controllers.Staff
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestsController : ControllerBase
    {
        //private readonly Application.Interfaces.Staff.Service.IQuestAdminService _questAdminService;
        //public QuestsController(Application.Interfaces.Staff.Service.IQuestAdminService questAdminService)
        //{
        //    _questAdminService = questAdminService;
        //}
        //[HttpGet]
        //public async Task<IActionResult> GetPaged(
        //    [FromQuery] int page = 1,
        //    [FromQuery] int pageSize = 20,
        //    [FromQuery] string? search = null,
        //    [FromQuery] string? filterField = null,
        //    [FromQuery] string? filterValue = null,
        //    [FromQuery] string? sort = null,
        //    CancellationToken ct = default)
        //{
        //    var paged = await _questAdminService.GetPagedAsync(
        //        page,
        //        pageSize,
        //        search,
        //        filterField,
        //        filterValue,
        //        sort,
        //        ct);

        //    return Ok(ApiResponse<PagedResult<QuestDTO>>.SuccessResponse(paged));
        //}

        ///// <summary>
        ///// Lấy chi tiết 1 Quest theo Id
        ///// </summary>
        //[HttpGet("{id:long}")]
        //public async Task<IActionResult> GetById(long id, CancellationToken ct)
        //{
        //    var dto = await _questAdminService.(id, ct);
        //    if (dto == null)
        //        return NotFound(ApiResponse<object>.ErrorResponse("Quest không tồn tại."));

        //    return Ok(ApiResponse<QuestDTO>.SuccessResponse(dto));
        //}

        ///// <summary>
        ///// Tạo Quest mới
        ///// </summary>
        //[HttpPost]
        //public async Task<IActionResult> Create([FromBody] QuestCreateDTO dto, CancellationToken ct)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        var errors = ModelState.Values
        //            .SelectMany(v => v.Errors)
        //            .Select(e => e.ErrorMessage)
        //            .ToArray();

        //        return BadRequest(ApiResponse<object>.ErrorResponse("Dữ liệu không hợp lệ.", errors));
        //    }

        //    var created = await _questAdminService.CreateAsync(dto, ct);
        //    return Ok(ApiResponse<QuestDTO>.SuccessResponse(created, "Tạo quest thành công."));
        //}

        ///// <summary>
        ///// Cập nhật Quest
        ///// </summary>
        //[HttpPut("{id:long}")]
        //public async Task<IActionResult> Update(long id, [FromBody] QuestUpdateDTO dto, CancellationToken ct)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        var errors = ModelState.Values
        //            .SelectMany(v => v.Errors)
        //            .Select(e => e.ErrorMessage)
        //            .ToArray();

        //        return BadRequest(ApiResponse<object>.ErrorResponse("Dữ liệu không hợp lệ.", errors));
        //    }

        //    await _questAdminService.UpdateAsync(id, dto, ct);
        //    return Ok(ApiResponse<object>.SuccessResponse(null, "Cập nhật quest thành công."));
        //}
    }
}
