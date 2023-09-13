using System.Text.Json;
using Microsoft.Extensions.Options;
using Pacer.Weather;

public class WeatherService : IWeatherService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    public WeatherService(HttpClient httpClient, IOptions<WeatherApiSettings> settings)
    {
        _httpClient = httpClient;
        _apiKey = settings.Value.ApiKey;
    }
    // This method is used to get weather data by latitude and longitude.
    public async Task<WeatherResponse> GetWeather(decimal latitude, decimal longitude)
    {
        string openWeatherUrl = $"http://api.openweathermap.org/data/2.5/weather?lat={latitude}&lon={longitude}&appid={_apiKey}&units=metric";

        HttpResponseMessage response = await _httpClient.GetAsync(openWeatherUrl);
        if (response.IsSuccessStatusCode)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            string content = await response.Content.ReadAsStringAsync();

            WeatherResponse weatherResponse = JsonSerializer.Deserialize<WeatherResponse>(content, options);
            return weatherResponse;
        }
        else
        {
            throw new Exception($"Failed to get weather: {response.StatusCode}");
        }
    }
    // This method is used to get weather data by location name.
    public async Task<WeatherResponse> GetWeatherByLocation(string location)
    {
        string openWeatherUrl = $"http://api.openweathermap.org/data/2.5/weather?q={location}&appid={_apiKey}&units=metric";

        HttpResponseMessage response = await _httpClient.GetAsync(openWeatherUrl);
        if (response.IsSuccessStatusCode)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            string content = await response.Content.ReadAsStringAsync();
            WeatherResponse weatherResponse = JsonSerializer.Deserialize<WeatherResponse>(content, options);
            return weatherResponse;
        }
        else
        {
            return null;
        }
    }
    // This method is used to get clothing advice based on weather data.
    public string GetClothingAdvice(decimal temperature, decimal windSpeed, decimal humidity, string weather)
    {
        // Basic clothing advice based on temperature.
        string advice = "Wear shorts and a tank top.";
        if (temperature < -10 || temperature > 40)
        {
            return "Dangerous temperatures detected. It is not advisable to run outdoors.";
        }
        if (windSpeed > 40)
        {
            return "High wind speeds detected. It is not advisable to run outdoors.";
        }

        if (temperature < 5)
        {
            advice = "Wear thermal leggings, a long-sleeve thermal shirt, a running jacket, gloves, and a beanie or ear warmers.";
        }
        else if (temperature < 10)
        {
            advice = "Wear thermal leggings, a long-sleeve thermal shirt, and a running jacket.";
            if (windSpeed > 5)
            {
                advice += " Consider adding gloves.";
            }
        }
        else if (temperature < 15)
        {
            advice = "Wear thermal leggings and a long-sleeve thermal shirt.";
        }
        else if (temperature < 20)
        {
            advice = "Wear shorts and a long-sleeve shirt.";
            if (humidity > 80)
            {
                advice += " Choose moisture-wicking fabrics.";
            }
        }
        else if (temperature < 25)
        {
            advice = "Wear shorts and a t-shirt.";
            if (humidity > 80)
            {
                advice += " Choose moisture-wicking fabrics.";
            }
        }

        // Adjustments based on other weather conditions.
        if (weather.ToLower().Contains("rain"))
        {
            advice += " Bring a waterproof running jacket.";
        }
        if (weather.ToLower().Contains("sun") || weather.ToLower().Contains("clear"))
        {
            advice += " Make sure to apply suncream.";
        }
        if (weather.ToLower().Contains("snow") || weather.ToLower().Contains("sleet"))
        {
            advice += " Wear waterproof shoes and be careful of slippery surfaces.";
        }
        if (weather.ToLower().Contains("thunderstorm"))
        {
            advice += " Consider postponing your run for safety.";
        }
        if (weather.ToLower().Contains("fog"))
        {
            advice += " Wear reflective gear for visibility.";
        }
        if (windSpeed > 10)
        {
            advice += " Wear a wind-resistant jacket.";
        }

        return advice;
    }


}
