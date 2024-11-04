using Microsoft.AspNetCore.Mvc;
using EmergencyCallServer.Models;
using EmergencyCallServer.utils;

namespace EmergencyCallServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VolunteersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public VolunteersController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult AddVolunteer([FromBody] Volunteer volunteer)
        {
            if (volunteer == null)
            {
                return BadRequest("Volunteer data is null.");
            }

            var volunteerModel = new VolunteerModel(_context);
            volunteerModel.AddVolunteer(volunteer);
            return CreatedAtAction(nameof(GetVolunteer), new { id = volunteer.Id }, volunteer);
        }

        [HttpGet("{id}")]
        public ActionResult<Volunteer> GetVolunteer(int id)
        {
            var volunteerModel = new VolunteerModel(_context);
            var volunteer = volunteerModel.GetVolunteer(id);

            if (volunteer == null)
            {
                return NotFound();
            }

            return Ok(volunteer);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateVolunteer(int id, [FromBody] Volunteer updatedVolunteer)
        {
            if (updatedVolunteer == null || updatedVolunteer.Id != id)
            {
                return BadRequest("Volunteer data is invalid.");
            }

            var volunteerModel = new VolunteerModel(_context);
            volunteerModel.UpdateVolunteer(updatedVolunteer);
            return NoContent(); // No content to return after a successful update
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteVolunteer(int id)
        {
            var volunteerModel = new VolunteerModel(_context);
            volunteerModel.DeleteVolunteer(id);
            return NoContent(); // No content to return after a successful delete
        }
    }
}
