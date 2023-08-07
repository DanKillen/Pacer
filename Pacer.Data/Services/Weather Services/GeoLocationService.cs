
using System.Text.Json;
using Microsoft.Extensions.Options;

public class GeoLocationService : ILocationService
{
    private readonly HttpClient _httpClient;
    private readonly string _accessKey;

    public GeoLocationService(HttpClient httpClient, IOptions<IpStackSettings> settings)
    {
        _httpClient = httpClient;
        _accessKey = settings.Value.ApiKey;
    }

    public async Task<IpStackResponse> GetLocationInfo(string ipAddress)
    {
        string ipstackUrl = $"http://api.ipstack.com/{ipAddress}?access_key={_accessKey}";

        HttpResponseMessage response = await _httpClient.GetAsync(ipstackUrl);
        if (response.IsSuccessStatusCode)
        {
            string content = await response.Content.ReadAsStringAsync();
            IpStackResponse ipStackResponse = JsonSerializer.Deserialize<IpStackResponse>(content);
            return ipStackResponse;
        }
        else
        {
            throw new Exception($"Failed to get location info: {response.StatusCode}");
        }
    }
}

