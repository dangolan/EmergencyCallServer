using EmergencyCallServer.Models;
using EmergencyCallServer.utils;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace EmergencyCallServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VolunteersController : ControllerBase
    {
        private readonly VolunteerModel _volunteerModel;

        public VolunteersController(VolunteerModel volunteerModel)
        {
            _volunteerModel = volunteerModel;
        }

        [HttpPost]
        public async Task<IActionResult> AddVolunteer([FromForm] Volunteer volunteer, [FromForm] PhotoUpload photo)
        {
            // Validate volunteer details
            if (volunteer == null)
            {
                return BadRequest("Volunteer data is missing.");
            }

            if (string.IsNullOrWhiteSpace(volunteer.FirstName))
            {
                return BadRequest("First name is required.");
            }

            if (string.IsNullOrWhiteSpace(volunteer.LastName))
            {
                return BadRequest("Last name is required.");
            }

            if (string.IsNullOrWhiteSpace(volunteer.UniqueIdNumber))
            {
                return BadRequest("Unique ID number is required.");
            }

            // Validate photo
            if (photo?.Photo != null && photo.Photo.ContentType != "image/png")
            {
               
                return BadRequest("Only PNG images are accepted.");
                
            }

            try
            {
                await _volunteerModel.AddVolunteerAsync(volunteer, photo.Photo);
                return CreatedAtAction(nameof(GetVolunteer), new { id = volunteer.Id }, volunteer);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while adding the volunteer: {ex.Message}");
            }
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<Volunteer>> GetVolunteer(int id)
        {
            try
            {
                var volunteer = await _volunteerModel.GetVolunteerAsync(id);
                if (volunteer == null)
                {
                    return NotFound($"Volunteer with ID {id} not found.");
                }
                return Ok(volunteer);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while retrieving the volunteer: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateVolunteer(int id, [FromForm] Volunteer updatedVolunteer, [FromForm] PhotoUpload newPhoto)
        {
            // Check if the volunteer data is valid
            if (updatedVolunteer == null || updatedVolunteer.Id != id)
            {
                return BadRequest("Volunteer data is invalid.");
            }

            if (string.IsNullOrWhiteSpace(updatedVolunteer.FirstName))
            {
                return BadRequest("First name is required.");
            }

            if (string.IsNullOrWhiteSpace(updatedVolunteer.LastName))
            {
                return BadRequest("Last name is required.");
            }

            if (string.IsNullOrWhiteSpace(updatedVolunteer.Phone))
            {
                return BadRequest("Phone number is required.");
            }

            if (string.IsNullOrWhiteSpace(updatedVolunteer.UniqueIdNumber))
            {
                return BadRequest("Unique ID number is required.");
            }

            // Validate the new photo if it exists
            if (newPhoto?.Photo != null && newPhoto.Photo.ContentType != "image/png")
            {
                return BadRequest("Only PNG images are accepted for the photo.");
            }

            try
            {
                await _volunteerModel.UpdateVolunteerAsync(updatedVolunteer, newPhoto?.Photo);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while updating the volunteer: {ex.Message}");
            }
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVolunteer(int id)
        {
            try
            {
                await _volunteerModel.DeleteVolunteerAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while deleting the volunteer: {ex.Message}");
            }
        }
    }
}
