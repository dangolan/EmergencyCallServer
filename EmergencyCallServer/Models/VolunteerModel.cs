using EmergencyCallServer.utils;
using Google.Cloud.Storage.V1;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EmergencyCallServer.Models
{
    public class VolunteerModel
    {
        private readonly ApplicationDbContext _context;
        private readonly GoogleCloudStorageModel _storageService;
        private readonly GeocodingModel _geocodingModel;

        public VolunteerModel(ApplicationDbContext context, GoogleCloudStorageModel storageService,GeocodingModel geocodingModel)
        {
            _context = context;
            _storageService = storageService;
            _geocodingModel = geocodingModel;
        }

        public async Task AddVolunteerAsync(Volunteer volunteer, IFormFile photo)
        {
            try
            {
                if (volunteer == null) throw new ArgumentNullException(nameof(volunteer), "Volunteer cannot be null.");
              

                // Check for duplicate UniqueIdNumber
                if (_context.Volunteers.Any(v => v.UniqueIdNumber == volunteer.UniqueIdNumber))
                {
                    throw new InvalidOperationException($"A volunteer with Unique ID number {volunteer.UniqueIdNumber} already exists.");
                }

                if (photo != null)
                {

                    // Upload the photo and set its URL in the volunteer's data
                    volunteer.PhotoUrl = await _storageService.UploadPhotoAsync(photo, volunteer.UniqueIdNumber);
                }
                else
                {
                    volunteer.PhotoUrl = "NoPhoto";
                }

                // Try to get coordinates for the address
                try
                {
                    if (!string.IsNullOrEmpty(volunteer.Address))
                    {
                        GeoPoint location = await _geocodingModel.GetCoordinatesAsync(volunteer.Address);
                        volunteer.Latitude = location.Latitude;
                        volunteer.Longitude = location.Longitude;
                    }
                }
                catch (Exception ex)
                {
                    // Log the exception if logging is set up, or handle the error as needed
                    throw new InvalidOperationException("Failed to retrieve coordinates for the provided address.", ex);
                }

                _context.Volunteers.Add(volunteer);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Log the exception if logging is set up
                throw new Exception("An error occurred while adding the volunteer.", ex);
            }
        }

        public async Task<Volunteer> GetVolunteerAsync(int id)
        {
            try
            {
                var volunteer = await _context.Volunteers.FindAsync(id);

                if (volunteer == null)
                {
                    throw new Exception($"Volunteer with ID {id} not found.");
                }

                return volunteer;
            }
            catch (Exception ex)
            {
                // Log the exception if logging is set up
                throw new Exception("An error occurred while retrieving the volunteer.", ex);
            }
        }

        public async Task UpdateVolunteerAsync(Volunteer updatedVolunteer, IFormFile newPhoto)
        {
            try
            {
                var existingVolunteer = await _context.Volunteers.FindAsync(updatedVolunteer.Id);
                if (existingVolunteer == null) throw new ArgumentException("Volunteer not found.");

                existingVolunteer.FirstName = updatedVolunteer.FirstName;
                existingVolunteer.LastName = updatedVolunteer.LastName;
                existingVolunteer.Phone = updatedVolunteer.Phone;
                existingVolunteer.Latitude = updatedVolunteer.Latitude;
                existingVolunteer.Longitude = updatedVolunteer.Longitude;
                existingVolunteer.Address = updatedVolunteer.Address;

                if (newPhoto != null)
                {
                    await _storageService.DeletePhotoAsync(existingVolunteer.UniqueIdNumber);
                    existingVolunteer.PhotoUrl = await _storageService.UploadPhotoAsync(newPhoto, existingVolunteer.UniqueIdNumber);
                }

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Log the exception if logging is set up
                throw new Exception("An error occurred while updating the volunteer.", ex);
            }
        }

        public async Task DeleteVolunteerAsync(int id)
        {
            try
            {
                var volunteer = await _context.Volunteers.FindAsync(id);
                if (volunteer == null) throw new ArgumentException("Volunteer not found.");

                await _storageService.DeletePhotoAsync(volunteer.UniqueIdNumber);

                _context.Volunteers.Remove(volunteer);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Log the exception if logging is set up
                throw new Exception("An error occurred while deleting the volunteer.", ex);
            }
        }

    }
}
