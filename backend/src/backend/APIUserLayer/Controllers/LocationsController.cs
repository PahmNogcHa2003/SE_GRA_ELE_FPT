using Application.Services.Location;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace APIUserLayer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocationsController : ControllerBase
    {
        private readonly ISender _sender;

        public LocationsController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet("provinces")]
        public async Task<IActionResult> GetProvinces()
        {
            var query = new GetProvincesQuery();
            return Ok(await _sender.Send(query));
        }

        [HttpGet("provinces/{provinceCode:int}/wards")]
        public async Task<IActionResult> GetWards(int provinceCode)
        {
            var query = new GetWardsByProvinceQuery(provinceCode);
            return Ok(await _sender.Send(query));
        }
    }
}
