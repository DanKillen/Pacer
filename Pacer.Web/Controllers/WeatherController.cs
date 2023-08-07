using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Pacer.Data.Services;
using Pacer.Data.Entities;
using System.Security.Claims;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Pacer.Web.Controllers;

public class WeatherController : BaseController
{
    private readonly ILocationService _locationService;
    private readonly IWeatherService _weatherService;

    public WeatherController(ILocationService locationService, IWeatherService weatherService, ILogger<WeatherController> logger) : base(logger)
    {
        _locationService = locationService;
        _weatherService = weatherService;
    }

    [HttpGet]
    public IActionResult Location()
    {
        // Show a page asking for the user's permission to use their location.
        return View();
    }
    // [HttpPost]
    // public async Task<IActionResult> Weather()
    // {
    //     //string ipAddress = HttpContext.Connection.RemoteIpAddress.ToString();
    //     string ipAddress = "194.168.4.100";
    //     Console.WriteLine($"IP Address: {ipAddress}");
    //     var locationInfo = await _locationService.GetLocationInfo(ipAddress);
    //     Console.WriteLine(locationInfo);
    //     Console.WriteLine($"City: {locationInfo.city}, Region: {locationInfo.region_name}, Country: {locationInfo.country_name}");

    //     var viewModel = new LocationViewModel
    //     {
    //         City = locationInfo.city,
    //         Region = locationInfo.region_name,
    //         Country = locationInfo.country_name,
    //     };
    //     Console.WriteLine($"City: {viewModel.City}, Region: {viewModel.Region}, Country: {viewModel.Country}");

    //     // Pass the viewModel to the Weather View
    //     return View("Weather", viewModel);

    [HttpPost]
    public async Task<IActionResult> Weather(decimal latitude, decimal longitude)
    {
        var weatherResponse = await _weatherService.GetWeather(latitude, longitude);
        Console.WriteLine(JsonSerializer.Serialize(weatherResponse));

        var weatherViewModel = new WeatherViewModel
        {
            Weather = weatherResponse.Weather[0].Main,
            Icon = weatherResponse.Weather[0].Icon,
            Description = weatherResponse.Weather[0].Description,
            Temperature = weatherResponse.Main.Temp,
            Humidity = weatherResponse.Main.Humidity,
            WindSpeed = weatherResponse.Wind.Speed,
            Sunset = DateTimeOffset.FromUnixTimeSeconds(weatherResponse.Sys.Sunset).UtcDateTime,
            Name = weatherResponse.Name,
            Country = weatherResponse.Sys?.Country,
            Advice = _weatherService.GetClothingAdvice(weatherResponse.Main.Temp, weatherResponse.Wind.Speed, weatherResponse.Main.Humidity, weatherResponse.Weather[0].Main)
        };

        if (!ModelState.IsValid)
        {
            // Log or print the errors
            foreach (var modelState in ModelState.Values)
            {
                foreach (var error in modelState.Errors)
                {
                    Console.WriteLine(error.ErrorMessage);
                }
            }
        }
        Console.WriteLine(weatherViewModel);
        return View(weatherViewModel);
    }

}

