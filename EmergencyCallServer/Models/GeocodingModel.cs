using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using EmergencyCallServer.utils;
using Newtonsoft.Json;

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
        try
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
                    var latitude = location["lat"].Value<decimal>();
                    var longitude = location["lng"].Value<decimal>();

                    return new GeoPoint(latitude, longitude);
                }
                else
                {
                    // No results for the address
                    throw new Exception("No results found for the specified address.");
                }
            }
            else
            {
                // Unsuccessful status code returned from the API
                throw new HttpRequestException($"Error fetching coordinates: {response.ReasonPhrase}");
            }
        }
        catch (HttpRequestException httpEx)
        {
            // Handle specific HTTP errors here
            throw new Exception("A network error occurred while fetching coordinates.", httpEx);
        }
        catch (JsonReaderException jsonEx)
        {
            // Handle issues with JSON parsing
            throw new Exception("An error occurred while parsing the geocoding response.", jsonEx);
        }
        catch (Exception ex)
        {
            // Handle all other exceptions
            throw new Exception("An unexpected error occurred while fetching coordinates.", ex);
        }
    }

}

