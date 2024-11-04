using System.Linq;
using EmergencyCallServer.utils;

namespace EmergencyCallServer.Models
{
    public class VolunteerModel
    {
        private readonly ApplicationDbContext _context;

        public VolunteerModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public void AddVolunteer(Volunteer volunteer)
        {
            if (volunteer == null)
            {
                throw new ArgumentNullException(nameof(volunteer), "Volunteer cannot be null.");
            }

            // Validate required fields
            if (string.IsNullOrWhiteSpace(volunteer.FirstName))
            {
                throw new ArgumentException("First name is required.", nameof(volunteer.FirstName));
            }

            if (string.IsNullOrWhiteSpace(volunteer.LastName))
            {
                throw new ArgumentException("Last name is required.", nameof(volunteer.LastName));
            }

            if (string.IsNullOrWhiteSpace(volunteer.UniqueIdNumber))
            {
                throw new ArgumentException("Unique ID number is required.", nameof(volunteer.UniqueIdNumber));
            }

            // Check for duplicate UniqueIdNumber
            var existingVolunteer = _context.Volunteers
                .FirstOrDefault(v => v.UniqueIdNumber == volunteer.UniqueIdNumber);
            if (existingVolunteer != null)
            {
                throw new InvalidOperationException($"A volunteer with Unique ID number {volunteer.UniqueIdNumber} already exists.");
            }

            // Add the volunteer to the context and save changes
            _context.Volunteers.Add(volunteer);
            _context.SaveChanges();
        }


        public Volunteer GetVolunteer(int id)
        {
            return _context.Volunteers.Find(id);
        }



        public void UpdateVolunteer(Volunteer updatedVolunteer)
        {
            var existingVolunteer = _context.Volunteers.Find(updatedVolunteer.Id);
            if (existingVolunteer != null)
            {
                existingVolunteer.FirstName = updatedVolunteer.FirstName;
                existingVolunteer.LastName = updatedVolunteer.LastName;
                existingVolunteer.Phone = updatedVolunteer.Phone;
                existingVolunteer.Latitude = updatedVolunteer.Latitude;
                existingVolunteer.Longitude = updatedVolunteer.Longitude;
                existingVolunteer.Address = updatedVolunteer.Address;
                existingVolunteer.Image = updatedVolunteer.Image;

                _context.SaveChanges();  // Save changes to the database
            }
        }

        public void DeleteVolunteer(int id)
        {
            var volunteer = _context.Volunteers.Find(id);
            if (volunteer != null)
            {
                _context.Volunteers.Remove(volunteer);
                _context.SaveChanges();  // Save changes to the database
            }
        }
    }
}
