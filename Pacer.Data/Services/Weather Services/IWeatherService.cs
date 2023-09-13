using Pacer.Weather;
public interface IWeatherService
{
    // This method is used to get weather data by geolocation data.
    Task<WeatherResponse> GetWeather(decimal latitude, decimal longitude);
    // This method is used to get weather data by location name.
    Task<WeatherResponse> GetWeatherByLocation(string location);
    // This method is used to get clothing advice based on weather data.
    string GetClothingAdvice(decimal temperature, decimal WindSpeed, decimal humidity, string weather);
}