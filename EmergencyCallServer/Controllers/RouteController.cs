using Microsoft.AspNetCore.Mvc;
using EmergencyCallServer.Models;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class RouteController : ControllerBase
{
    private readonly RouteModel _routeModel;

    public RouteController(RouteModel routeModel)
    {
        _routeModel = routeModel;
    }

    [HttpGet("GetRoute")]
    public async Task<IActionResult> GetRoute([FromQuery] double startLat, [FromQuery] double startLon, [FromQuery] double endLat, [FromQuery] double endLon)
    {
        try
        {
            var routeData = await _routeModel.GetRouteAsync(startLat, startLon, endLat, endLon);
            return Ok(routeData);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
