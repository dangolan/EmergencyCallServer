using Microsoft.AspNetCore.Mvc;

namespace EmergencyCallServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GeocodingController : ControllerBase
    {
        private readonly GeocodingModel _geocodingService;

        public GeocodingController(GeocodingModel geocodingService)
        {
            _geocodingService = geocodingService;
        }

        [HttpGet("coordinates")]
        public async Task<IActionResult> GetCoordinates(string address)
        {
            try
            {
                var coordinates = await _geocodingService.GetCoordinatesAsync(address);
                return Ok(coordinates);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while getting coordinates: {ex.Message}");
            }
        }
    }
}
