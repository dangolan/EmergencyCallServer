using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EmergencyCallServer.utils;
using Newtonsoft.Json.Linq;

namespace EmergencyCallServer.Models
{
    public class EventVolunteersModel
    {
        private readonly VolunteersDB _context;
        private readonly RouteModel _routeModel;

        public EventVolunteersModel(VolunteersDB context, RouteModel routeModel)
        {
            _context = context;
            _routeModel = routeModel;
        }

        public async Task<List<EventVolunteer>> GetClosestEventVolunteersAsync(double eventLat, double eventLon)
        {
            // Step 1: Sort volunteers by Euclidean distance and select the closest 5
            var closestVolunteers = _context.Volunteers
                .OrderBy(v => Math.Sqrt(Math.Pow((double)(v.Latitude - (decimal)eventLat), 2) +
                                        Math.Pow((double)(v.Longitude - (decimal)eventLon), 2)))
                .Take(5)
                .ToList();

            // Step 2: Create a list to store EventVolunteer objects
            var eventVolunteers = new List<EventVolunteer>();

            // Step 3: Get route details for each volunteer and populate EventVolunteer objects
            foreach (var volunteer in closestVolunteers)
            {
                try
                {
                    // Retrieve route data
                    var routeResponse = await _routeModel.GetRouteAsync((double)volunteer.Latitude, (double)volunteer.Longitude, eventLat, eventLon);
                    var routeJson = JObject.Parse(routeResponse);

                    // Extract distance, duration, and coordinates
                    double distance = routeJson["features"][0]["properties"]["segments"][0]["distance"].Value<double>();
                    double duration = routeJson["features"][0]["properties"]["segments"][0]["duration"].Value<double>();
                    var coordinates = routeJson["features"][0]["geometry"]["coordinates"]
                        .Select(coord => coord.Select(c => c.Value<double>()).ToList())
                        .ToList();

                    // Create and add EventVolunteer object to the list
                    var eventVolunteer = new EventVolunteer(volunteer.Id, distance, duration, coordinates);
                    eventVolunteers.Add(eventVolunteer);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error retrieving route for Volunteer {volunteer.Id}: {ex.Message}");
                }
            }

            return eventVolunteers;
        }
    }
}
