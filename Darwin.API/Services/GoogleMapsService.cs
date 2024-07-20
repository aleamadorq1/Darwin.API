using Newtonsoft.Json.Linq;

namespace Darwin.API.Services;

public class GoogleMapsService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public GoogleMapsService(HttpClient httpClient, string apiKey)
    {
        _httpClient = httpClient;
        _apiKey = apiKey;
    }

    public async Task<double> GetDrivingDistanceAsync(string origin, string destination)
    {
        var url = $"https://maps.googleapis.com/maps/api/distancematrix/json?origins={origin}&destinations={destination}&key={_apiKey}&loading=async";

        var response = await _httpClient.GetStringAsync(url);
        var responseObject = JObject.Parse(response);

        var rows = responseObject["rows"]?[0];
        var elements = rows?["elements"]?[0];
        var distance = elements?["distance"]?["value"]?.ToObject<double>() ?? 0;

        return distance / 1000.0; // Convert meters to kilometers
    }
}
