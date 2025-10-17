using Application.Common;
using Application.DTOs;
using Application.Interfaces.Staff.Service;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AdminLayer.Controllers.Staff
{
    [Route("api/[controller]")]
    [ApiController]
    public class TagsController : ControllerBase
    {
        private readonly ITagService _tagService;

        public TagsController(ITagService tagService)
        {
            _tagService = tagService;
        }

        // GET: api/tags
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<TagDTO>>>> GetTags(
            [FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null,
            [FromQuery] string? filterField = null, [FromQuery] string? filterValue = null, [FromQuery] string? sortOrder = null)
        {
            var pagedTags = await _tagService.GetPagedAsync(page, pageSize, search, filterField, filterValue, sortOrder);
            return Ok(ApiResponse<PagedResult<TagDTO>>.SuccessResponse(pagedTags, "Fetched tags successfully"));
        }

        // GET: api/tags/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<TagDTO>>> GetTagById(long id)
        {
            var tag = await _tagService.GetAsync(id);
            return Ok(ApiResponse<TagDTO>.SuccessResponse(tag, "Fetched tag successfully"));
        }

        // POST: api/tags
        [HttpPost]
        public async Task<ActionResult<ApiResponse<TagDTO>>> CreateTag([FromBody] TagDTO createDto)
        {
            var createdTag = await _tagService.CreateAsync(createDto);
            var response = ApiResponse<TagDTO>.SuccessResponse(createdTag, "Tag created successfully");
            return CreatedAtAction(nameof(GetTagById), new { id = createdTag.Id }, response);
        }

        // PUT: api/tags/5
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> UpdateTag(long id, [FromBody] TagDTO updateDto)
        {
            if (updateDto.Id != 0 && id != updateDto.Id)
            {
                var errorResponse = ApiResponse<object>.ErrorResponse("Route ID and Body ID do not match", new[] { "Invalid ID parameter" });
                return BadRequest(errorResponse);
            }

            await _tagService.UpdateAsync(id, updateDto);
            return Ok(ApiResponse<object>.SuccessResponse(null, "Tag updated successfully"));
        }

        // DELETE: api/tags/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> DeleteTag(long id)
        {
            await _tagService.DeleteAsync(id);
            return Ok(ApiResponse<object>.SuccessResponse(null, "Tag deleted successfully"));
        }
    }
}