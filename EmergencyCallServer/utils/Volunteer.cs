namespace EmergencyCallServer.utils
{
    public class Volunteer
    {
        // Database-assigned primary key
        public int Id { get; set; }

        // Unique ID number for the volunteer, managed separately
        public string UniqueIdNumber { get; set; }

        // Volunteer personal information
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }

        // Location data
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public string Address { get; set; }

        // URL for the volunteer's image
        public string PhotoUrl { get; set; }


        // Constructor for convenience (optional)
        public Volunteer()
        {
        }

        public Volunteer(string uniqueIdNumber, string firstName, string lastName, string phone, decimal latitude, decimal longitude, string address, string photoUrl)
        {
            UniqueIdNumber = uniqueIdNumber;
            FirstName = firstName;
            LastName = lastName;
            Phone = phone;
            Latitude = latitude;
            Longitude = longitude;
            Address = address;
            PhotoUrl = photoUrl;
        }
    }
}
