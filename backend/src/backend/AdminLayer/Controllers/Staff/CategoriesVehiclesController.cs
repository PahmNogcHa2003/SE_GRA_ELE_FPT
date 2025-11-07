using Application.Common;
using Application.DTOs;
using Application.DTOs.CategoriesVehicle;
using Application.Interfaces.Staff.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AdminLayer.Controllers.Staff
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "AdminOnly")]
    public class CategoriesVehiclesController : ControllerBase
    {
        private readonly ICategoriesVehicleService _categoriesVehicleService;

        public CategoriesVehiclesController(ICategoriesVehicleService categoriesVehicleService)
        {
            _categoriesVehicleService = categoriesVehicleService;
        }

        // GET: api/categoriesvehicles
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<CategoriesVehicleDTO>>>> GetCategoriesVehicles(
            [FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null,
            [FromQuery] string? filterField = null, [FromQuery] string? filterValue = null, [FromQuery] string? sortOrder = null)
        {
            var pagedCategories = await _categoriesVehicleService.GetPagedAsync(page, pageSize, search, filterField, filterValue, sortOrder);
            return Ok(ApiResponse<PagedResult<CategoriesVehicleDTO>>.SuccessResponse(pagedCategories, "Fetched categories of vehicles successfully"));
        }

        // GET: api/categoriesvehicles/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<CategoriesVehicleDTO>>> GetCategoryVehicleById(long id)
        {
            var category = await _categoriesVehicleService.GetAsync(id);
            // Service will throw KeyNotFoundException if not found, which will be handled by middleware.
            return Ok(ApiResponse<CategoriesVehicleDTO>.SuccessResponse(category, "Fetched category of vehicle successfully"));
        }

        // POST: api/categoriesvehicles
        [HttpPost]
        public async Task<ActionResult<ApiResponse<CategoriesVehicleDTO>>> CreateCategoryVehicle([FromBody] CategoriesVehicleDTO createDto)
        {
            // ModelState is automatically validated by [ApiController]
            var createdCategory = await _categoriesVehicleService.CreateAsync(createDto);
            var response = ApiResponse<CategoriesVehicleDTO>.SuccessResponse(createdCategory, "Category of vehicle created successfully");
            return CreatedAtAction(nameof(GetCategoryVehicleById), new { id = createdCategory.Id }, response);
        }

        // PUT: api/categoriesvehicles/5
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> UpdateCategoryVehicle(long id, [FromBody] CategoriesVehicleDTO updateDto)
        {
            if (updateDto.Id != 0 && id != updateDto.Id)
            {
                var errorResponse = ApiResponse<object>.ErrorResponse("Route ID and Body ID do not match", new[] { "Invalid ID parameter" });
                return BadRequest(errorResponse);
            }

            await _categoriesVehicleService.UpdateAsync(id, updateDto);
            return Ok(ApiResponse<object>.SuccessResponse(null, "Category of vehicle updated successfully"));
        }

        // DELETE: api/categoriesvehicles/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> DeleteCategoryVehicle(long id)
        {
            await _categoriesVehicleService.DeleteAsync(id);
            return Ok(ApiResponse<object>.SuccessResponse(null, "Category of vehicle deleted successfully"));
        }
    }
}