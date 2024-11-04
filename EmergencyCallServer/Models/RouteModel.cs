using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace EmergencyCallServer.Models {
    public class RouteModel
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey = "5b3ce3597851110001cf6248043f4755114f46d29a7ef9cba39ed8a5";

        public RouteModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> GetRouteAsync(double startLat, double startLon, double endLat, double endLon)
        {
            string url = $"https://api.openrouteservice.org/v2/directions/driving-car?api_key={_apiKey}&start={startLon},{startLat}&end={endLon},{endLat}";

            HttpResponseMessage response = await _httpClient.GetAsync(url);

       

            if (response.IsSuccessStatusCode)
            {
                string responseData = await response.Content.ReadAsStringAsync();

                // Print the response to the console
                Console.WriteLine("Received response from OpenRouteService API:");
                Console.WriteLine(responseData);

                return responseData;
            }
            else
            {
                string errorResponse = await response.Content.ReadAsStringAsync();

                // Print the error response to the console
                Console.WriteLine("Error response from OpenRouteService API:");
                Console.WriteLine(errorResponse);

                throw new Exception("Error retrieving route from OpenRouteService.");
            }
        }

    }
}


