using Application.Common;
using Application.DTOs;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace APIUserLayer.Controllers.User
{
    [Route("api/[controller]")]
    [ApiController]
    public class NewsController : ControllerBase
    {
        private readonly Application.Interfaces.User.Service.INewsService _newsService;
        public NewsController(Application.Interfaces.User.Service.INewsService newsService)
        {
            _newsService = newsService;
        }
        [HttpGet]
        public async Task<ActionResult<Application.Common.ApiResponse<Application.Common.PagedResult<Application.DTOs.NewsDTO>>>> Get(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null,
            [FromQuery] string? filterField = null,
            [FromQuery] string? filterValue = null,
            [FromQuery] string? sortOrder = null,
            CancellationToken ct = default)
        {
            var data = await _newsService.GetPagedAsync(page, pageSize, search, filterField, filterValue, sortOrder, ct);
            return Ok(Application.Common.ApiResponse<Application.Common.PagedResult<Application.DTOs.NewsDTO>>.SuccessResponse(data, "Fetched news"));
        }
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
        [HttpGet("related")]
        public async Task<ActionResult<Application.Common.ApiResponse<IEnumerable<Application.DTOs.NewsDTO>>>> GetRelatedNews(
            [FromQuery] long newsId,
            [FromQuery] int limit = 5,
            CancellationToken ct = default)
        {
            var data = await _newsService.GetRelatedNewsAsync(newsId, limit, ct);
            return Ok(Application.Common.ApiResponse<IEnumerable<Application.DTOs.NewsDTO>>.SuccessResponse(data, "Fetched related news"));
        }
    }
}
