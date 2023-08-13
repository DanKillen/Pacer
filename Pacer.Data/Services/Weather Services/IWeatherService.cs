using Pacer.Weather;
public interface IWeatherService
{
    Task<WeatherResponse> GetWeather(decimal latitude, decimal longitude);
    Task<WeatherResponse> GetWeatherByLocation(string location);
    string GetClothingAdvice(decimal temperature, decimal WindSpeed, decimal humidity, string weather);
}