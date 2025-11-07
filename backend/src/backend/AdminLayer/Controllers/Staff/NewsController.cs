using Application.Common;
using Application.DTOs;
using Application.DTOs.New;
using Application.Interfaces.Staff.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic; // Cần cho KeyNotFoundException
using System.Threading.Tasks;

namespace AdminLayer.Controllers.Staff
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "AdminOnly")]
    public class NewsController : ControllerBase
    {
        private readonly INewsService _newsService;

        public NewsController(INewsService newsService)
        {
            _newsService = newsService;
        }

        /// <summary>
        /// Lấy danh sách bài viết có phân trang, tìm kiếm và lọc.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<NewsDTO>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<PagedResult<NewsDTO>>>> GetNews(
            [FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null,
            [FromQuery] string? filterField = null, [FromQuery] string? filterValue = null, [FromQuery] string? sortOrder = null)
        {
            var pagedNews = await _newsService.GetPagedAsync(page, pageSize, search, filterField, filterValue, sortOrder);
            return Ok(ApiResponse<PagedResult<NewsDTO>>.SuccessResponse(pagedNews, "Fetched news successfully."));
        }

        /// <summary>
        /// Lấy chi tiết một bài viết theo ID.
        /// </summary>
        /// <param name="id">ID của bài viết.</param>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<NewsDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<NewsDTO>>> GetNewsById(long id)
        {
            var newsItem = await _newsService.GetAsync(id);

            if (newsItem == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse($"News with ID {id} not found.", new[] { "ID not found" }));
            }

            return Ok(ApiResponse<NewsDTO>.SuccessResponse(newsItem, "Fetched news item successfully."));
        }

        /// <summary>
        /// Tạo một bài viết mới.
        /// </summary>
        /// <param name="createDto">Đối tượng chứa thông tin bài viết mới.</param>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<NewsDTO>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<NewsDTO>>> CreateNews([FromBody] NewsDTO createDto)
        {
            // ModelState đã được [ApiController] tự động kiểm tra
            var createdNews = await _newsService.CreateAsync(createDto);
            var response = ApiResponse<NewsDTO>.SuccessResponse(createdNews, "News item created successfully.");
            return CreatedAtAction(nameof(GetNewsById), new { id = createdNews.Id }, response);
        }

        /// <summary>
        /// Cập nhật một bài viết đã có.
        /// </summary>
        /// <param name="id">ID của bài viết cần cập nhật.</param>
        /// <param name="updateDto">Đối tượng chứa thông tin cập nhật.</param>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateNews(long id, [FromBody] NewsDTO updateDto)
        {
            if (updateDto.Id != 0 && id != updateDto.Id)
            {
                var errorResponse = ApiResponse<object>.ErrorResponse("Route ID and Body ID do not match.", new[] { "Invalid ID parameter" });
                return BadRequest(errorResponse);
            }

            try
            {
                await _newsService.UpdateAsync(id, updateDto);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<object>.ErrorResponse(ex.Message, new[] { "ID not found" }));
            }

            return NoContent(); // Trả về 204 No Content khi thành công
        }

        /// <summary>
        /// Xóa một bài viết.
        /// </summary>
        /// <param name="id">ID của bài viết cần xóa.</param>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteNews(long id)
        {
            try
            {
                await _newsService.DeleteAsync(id);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<object>.ErrorResponse(ex.Message, new[] { "ID not found" }));
            }

            return NoContent(); // Trả về 204 No Content khi thành công
        }
    }
}