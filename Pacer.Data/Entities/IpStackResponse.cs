using System.Text.Json.Serialization;

public class IpStackResponse
{
    [JsonPropertyName("ip")]
    public string Ip { get; set; }

    [JsonPropertyName("city")]
    public string City { get; set; }

    [JsonPropertyName("region_name")]
    public string RegionName { get; set; }

    [JsonPropertyName("country_name")]
    public string CountryName { get; set; }

    [JsonPropertyName("zip")]
    public string Zip { get; set; }

    [JsonPropertyName("location")]
    public Location Location { get; set; }
}

public class Location
{
    [JsonPropertyName("geoname_id")]
    public string GeonameId { get; set; }

    [JsonPropertyName("country_flag")]
    public string CountryFlag { get; set; }

    [JsonPropertyName("country_flag_emoji")]
    public string CountryFlagEmoji { get; set; }
}
