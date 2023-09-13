namespace Pacer.Weather;
// Weather response fields that are populated for weather data
public class WeatherResponse
{
    public Main Main { get; set; }
    public List<Weather> Weather { get; set; }
    public Wind Wind { get; set; }
    public Sys Sys { get; set; }
    public string Name { get; set; }
}

public class Main
{
    public decimal Humidity { get; set; }
    public decimal Temp { get; set; }
}

public class Weather
{
    public string Main { get; set; }
    public string Description { get; set; }
    public string Icon { get; set; }
}

public class Wind
{
    public decimal Speed { get; set; }
}

public class Sys
{
    public long Sunset { get; set; }
    public string Country { get; set; }
}
