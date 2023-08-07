public interface IWeatherService
{
    Task<WeatherResponse> GetWeather(decimal latitude, decimal longitude);

    string GetClothingAdvice(decimal temperature, decimal WindSpeed, decimal humidity, string weather);
}