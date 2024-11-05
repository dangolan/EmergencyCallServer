using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using EmergencyCallServer.Models;
using EmergencyCallServer.utils;


namespace EmergencyCallServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PhotoController : ControllerBase
    {
        private readonly GoogleCloudStorageModel _storageService;

        public PhotoController(GoogleCloudStorageModel storageService)
        {
            _storageService = storageService;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadPhoto([FromForm] PhotoUpload model, string VolunteerId)
        {
            if (model.Photo == null || model.Photo.Length == 0)
            {
                return BadRequest("No photo uploaded.");
            }

            var fileName = VolunteerId;

            try
            {
                // Attempt to upload the photo
                var photoUrl = await _storageService.UploadPhotoAsync(model.Photo, fileName);
                return Ok(new { Url = photoUrl });
            }
            catch (ArgumentException ex)
            {
                // Handle validation errors, such as unsupported file types
                return BadRequest(ex.Message);
            }
            catch (Google.GoogleApiException ex)
            {
                // Handle Google Cloud specific errors
                return StatusCode(500, "An error occurred while uploading to Google Cloud Storage.");
            }
            catch (Exception ex)
            {
                // Handle any other unexpected errors
                return StatusCode(500, "An unexpected error occurred.");
            }
        }


        [HttpDelete("delete/{fileName}")]
        public async Task<IActionResult> DeletePhoto(string fileName)
        {
            bool isDeleted = await _storageService.DeletePhotoAsync(fileName);

            if (!isDeleted)
            {
                return NotFound(new { Message = "Photo not found or could not be deleted." });
            }

            return Ok(new { Message = "Photo deleted successfully." });
        }
    }
}
