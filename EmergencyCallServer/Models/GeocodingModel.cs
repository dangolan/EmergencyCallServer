using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using EmergencyCallServer.utils;

public class GeocodingModel
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public GeocodingModel(string apiKey)
    {  
        _apiKey = apiKey; 
        _httpClient = new HttpClient();
    }

    public async Task<GeoPoint> GetCoordinatesAsync(string address)
    {
        var requestUri = $"https://maps.googleapis.com/maps/api/geocode/json?address={Uri.EscapeDataString(address)}&key={_apiKey}";

        var response = await _httpClient.GetAsync(requestUri);

        if (response.IsSuccessStatusCode)
        {
            var jsonResponse = await response.Content.ReadAsStringAsync();
            var json = JObject.Parse(jsonResponse);

            if (json["results"].HasValues)
            {
                var location = json["results"][0]["geometry"]["location"];
                var latitude = location["lat"].Value<double>();
                var longitude = location["lng"].Value<double>();

                return new GeoPoint(latitude, longitude);
            }
            else
            {
                throw new Exception("No results found for the specified address.");
            }
        }

        throw new Exception($"Error fetching coordinates: {response.ReasonPhrase}");
    }
}

