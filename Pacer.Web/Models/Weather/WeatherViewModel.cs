using System.Text.Json.Serialization;

public class WeatherViewModel
{
    [JsonPropertyName("name")]
    public string Name { get; set; }
    [JsonPropertyName("main")]
    public string Weather { get; set; }
    [JsonPropertyName("description")]
    public string Description { get; set; }
    [JsonPropertyName("icon")]
    public string Icon { get; set; }
    [JsonPropertyName("humidity")]
    public decimal Humidity { get; set; } = 50.0M;
    [JsonPropertyName("temp")]
    public decimal Temperature { get; set; } = 50.0M;
    [JsonPropertyName("speed")]
    public decimal WindSpeed { get; set; } = 50.0M;
    [JsonPropertyName("sunset")]
    public DateTime Sunset { get; set; } = DateTime.Now;

    [JsonPropertyName("country")]
    public string Country { get; set; }
    public string Advice { get; set; }
}