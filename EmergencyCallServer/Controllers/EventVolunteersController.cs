using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EmergencyCallServer.Models;
using Microsoft.AspNetCore.Mvc;

namespace EmergencyCallServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventVolunteersController : ControllerBase
    {
        private readonly EventVolunteersModel _eventVolunteersModel;

        public EventVolunteersController(EventVolunteersModel eventVolunteersModel)
        {
            _eventVolunteersModel = eventVolunteersModel;
        }

        /// <summary>
        /// Gets the 20 closest volunteers to a specified event location.
        /// </summary>
        /// <param name="eventLat">The latitude of the event location.</param>
        /// <param name="eventLon">The longitude of the event location.</param>
        /// <returns>A list of 20 EventVolunteer objects sorted by distance from the event.</returns>
        [HttpGet("closestVolunteers")]
        public async Task<IActionResult> GetClosestVolunteers([FromQuery] double eventLat, [FromQuery] double eventLon)
        {
            try
            {
                // Get the closest volunteers
                List<EventVolunteer> closestVolunteers = await _eventVolunteersModel.GetClosestEventVolunteersAsync(eventLat, eventLon);

                // Return the list of closest volunteers as a JSON response
                return Ok(closestVolunteers);
            }
            catch (Exception ex)
            {
                // Log the exception (optional) and return a 500 status code with error message
                Console.WriteLine($"Error fetching closest volunteers: {ex.Message}");
                return StatusCode(500, "An error occurred while fetching the closest volunteers.");
            }
        }
    }
}
