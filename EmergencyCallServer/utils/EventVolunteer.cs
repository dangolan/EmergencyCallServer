using System.Collections.Generic;

namespace EmergencyCallServer.Models
{
    public class EventVolunteer
    {
        public int VolunteerId { get; set; }        // ID of the volunteer
        public double Distance { get; set; }        // Distance to the event in meters
        public double Duration { get; set; }        // Duration to the event in seconds
        public List<List<double>> RouteCoordinates { get; set; }   // List of route coordinates (latitude, longitude)

        public EventVolunteer(int volunteerId, double distance, double duration, List<List<double>> routeCoordinates)
        {
            VolunteerId = volunteerId;
            Distance = distance;
            Duration = duration;
            RouteCoordinates = routeCoordinates;
        }
    }
}
