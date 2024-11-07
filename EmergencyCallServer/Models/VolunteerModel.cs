using EmergencyCallServer.utils;
using Google.Cloud.Storage.V1;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EmergencyCallServer.Models
{
    public class VolunteerModel
    {
        private readonly VolunteersDB _volunteersDB;
        private readonly GoogleCloudStorageModel _storageService;
        private readonly GeocodingModel _geocodingModel;

        public VolunteerModel(VolunteersDB volunteersDB, GoogleCloudStorageModel storageService,GeocodingModel geocodingModel)
        {
            _volunteersDB = volunteersDB;
            _storageService = storageService;
            _geocodingModel = geocodingModel;
        }

        public async Task AddVolunteerAsync(Volunteer volunteer, IFormFile photo)
        {
            try
            {
                if (volunteer == null) throw new ArgumentNullException(nameof(volunteer), "Volunteer cannot be null.");

                // Ensure UniqueIdNumber contains only digits
                if (!Regex.IsMatch(volunteer.UniqueIdNumber, @"^\d+$"))
                {
                    throw new ArgumentException("UniqueIdNumber must contain only numeric characters.");
                }


                // Check for duplicate UniqueIdNumber
                if (_volunteersDB.Volunteers.Any(v => v.UniqueIdNumber == volunteer.UniqueIdNumber))
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

                ///Try to get coordinates for the address
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

                _volunteersDB.Volunteers.Add(volunteer);
                await _volunteersDB.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Log the exception if logging is set up
                throw new Exception(ex.Message, ex);
            }
        }
        public async Task<List<Volunteer>> GetAllVolunteersAsync()
        {
            var volunteers = await _volunteersDB.Volunteers.ToListAsync();

            if (volunteers == null || !volunteers.Any())
            {
                throw new Exception("No volunteers found in the database.");
            }

            return volunteers;

        }


        public async Task<Volunteer> GetVolunteerAsync(int id)
        {
            try
            {
                var volunteer = await _volunteersDB.Volunteers.FindAsync(id);

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
                if (updatedVolunteer == null) throw new ArgumentNullException(nameof(updatedVolunteer), "Updated volunteer data cannot be null.");

                // Check if the volunteer exists in the database
                var existingVolunteer = await _volunteersDB.Volunteers.FindAsync(updatedVolunteer.Id);
                if (existingVolunteer == null) throw new ArgumentException("Volunteer not found.");

                // Ensure UniqueIdNumber contains only digits
                if (!Regex.IsMatch(updatedVolunteer.UniqueIdNumber, @"^\d+$"))
                {
                    throw new ArgumentException("UniqueIdNumber must contain only numeric characters.");
                }

                // Check for duplicate UniqueIdNumber if it has changed
                if (_volunteersDB.Volunteers.Any(v => v.UniqueIdNumber == updatedVolunteer.UniqueIdNumber && v.Id != updatedVolunteer.Id))
                {
                    throw new InvalidOperationException($"A volunteer with Unique ID number {updatedVolunteer.UniqueIdNumber} already exists.");
                }

                // Update volunteer data
                existingVolunteer.FirstName = updatedVolunteer.FirstName;
                existingVolunteer.LastName = updatedVolunteer.LastName;
                existingVolunteer.Phone = updatedVolunteer.Phone;
                existingVolunteer.Address = updatedVolunteer.Address;

                // Try to update coordinates if address has changed
                try
                {
                    if (!string.IsNullOrEmpty(updatedVolunteer.Address) && updatedVolunteer.Address != existingVolunteer.Address)
                    {
                        GeoPoint location = await _geocodingModel.GetCoordinatesAsync(updatedVolunteer.Address);
                        existingVolunteer.Latitude = location.Latitude;
                        existingVolunteer.Longitude = location.Longitude;
                    }
                }
                catch (Exception ex)
                {
                    // Handle errors with the geocoding service gracefully
                    throw new InvalidOperationException("Failed to retrieve coordinates for the updated address.", ex);
                }

                // If there’s a new photo, update the photo URL
                if (newPhoto != null)
                {
                    await _storageService.DeletePhotoAsync(existingVolunteer.UniqueIdNumber);
                    existingVolunteer.PhotoUrl = await _storageService.UploadPhotoAsync(newPhoto, existingVolunteer.UniqueIdNumber);
                }
                else if (string.IsNullOrEmpty(existingVolunteer.PhotoUrl))
                {
                    // If no new photo and no existing photo, set a default
                    existingVolunteer.PhotoUrl = "NoPhoto";
                }

                // Save changes
                await _volunteersDB.SaveChangesAsync();
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
                var volunteer = await _volunteersDB.Volunteers.FindAsync(id);
                if (volunteer == null) throw new ArgumentException("Volunteer not found.");

                _volunteersDB.Volunteers.Remove(volunteer);
                await _volunteersDB.SaveChangesAsync();

                await _storageService.DeletePhotoAsync(volunteer.UniqueIdNumber);
            }
            catch (Exception ex)
            {
                // Log the exception if logging is set up
                throw new Exception("An error occurred while deleting the volunteer.", ex);
            }
        }

    }
}
