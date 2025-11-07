using Application.Common;
using Application.DTOs;
using Application.DTOs.Vehicle;
using Application.Interfaces.Staff.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QRCoder;
using System.Threading.Tasks;

namespace AdminLayer.Controllers.Staff
{
    /// <summary>
    /// Manages Vehicle-related APIs.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "AdminOnly")]
    public class VehiclesController : ControllerBase
    {
        // Service to handle vehicle business logic.
        private readonly IVehicleService _vehicleService;

        /// <summary>
        /// Injects the vehicle service via constructor.
        /// </summary>
        public VehiclesController(IVehicleService vehicleService)
        {
            _vehicleService = vehicleService;
        }

        /// <summary>
        /// Gets a paginated list of vehicles with options for searching, filtering, and sorting.
        /// </summary>
        // GET: api/vehicles
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<VehicleDTO>>>> GetVehicles(
            [FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null,
            [FromQuery] string? filterField = null, [FromQuery] string? filterValue = null, [FromQuery] string? sortOrder = null)
        {
            var pagedVehicles = await _vehicleService.GetPagedAsync(page, pageSize, search, filterField, filterValue, sortOrder);
            return Ok(ApiResponse<PagedResult<VehicleDTO>>.SuccessResponse(pagedVehicles, "Fetched vehicles successfully"));
        }

        // GET: api/staff/Vehicles/{id}/qrcode
        [HttpGet("{id}/qrcode")]
        public IActionResult GenerateQrCodeForVehicle(long id)
        {
           
            string dataToEncode = id.ToString();

            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(dataToEncode, QRCodeGenerator.ECCLevel.Q);
            PngByteQRCode qrCode = new PngByteQRCode(qrCodeData);
            byte[] qrCodeImageBytes = qrCode.GetGraphic(20); 

            return File(qrCodeImageBytes, "image/png");
        }

        /// <summary>
        /// Gets a specific vehicle by its ID.
        /// </summary>
        // GET: api/vehicles/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<VehicleDTO>>> GetVehicleById(long id)
        {
            var vehicle = await _vehicleService.GetAsync(id);
            // Service throws KeyNotFoundException if not found, which is handled by global middleware.
            return Ok(ApiResponse<VehicleDTO>.SuccessResponse(vehicle, "Fetched vehicle successfully"));
        }

        /// <summary>
        /// Creates a new vehicle.
        /// </summary>
        // POST: api/vehicles
        [HttpPost]
        public async Task<ActionResult<ApiResponse<VehicleDTO>>> CreateVehicle([FromBody] VehicleDTO createDto)
        {
            // [ApiController] handles ModelState validation automatically.
            var createdVehicle = await _vehicleService.CreateAsync(createDto);
            var response = ApiResponse<VehicleDTO>.SuccessResponse(createdVehicle, "Vehicle created successfully");

            // Return 201 Created with a location header pointing to the new resource.
            return CreatedAtAction(nameof(GetVehicleById), new { id = createdVehicle.Id }, response);
        }

        /// <summary>
        /// Updates an existing vehicle.
        /// </summary>
        // PUT: api/vehicles/5
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> UpdateVehicle(long id, [FromBody] VehicleDTO updateDto)
        {
            // Validate that the route ID matches the ID in the request body.
            if (updateDto.Id != 0 && id != updateDto.Id)
            {
                var errorResponse = ApiResponse<object>.ErrorResponse("Route ID and Body ID do not match", new[] { "Invalid ID parameter" });
                return BadRequest(errorResponse);
            }

            await _vehicleService.UpdateAsync(id, updateDto);
            return Ok(ApiResponse<object>.SuccessResponse(null, "Vehicle updated successfully"));
        }

        /// <summary>
        /// Deletes a vehicle by its ID.
        /// </summary>
        // DELETE: api/vehicles/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> DeleteVehicle(long id)
        {
            await _vehicleService.DeleteAsync(id);
            return Ok(ApiResponse<object>.SuccessResponse(null, "Vehicle deleted successfully"));
        }
    }
}