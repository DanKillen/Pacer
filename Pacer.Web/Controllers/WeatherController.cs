using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Pacer.Data.Services;
using Pacer.Data.Entities;
using System.Security.Claims;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Globalization;

namespace Pacer.Web.Controllers;

public class WeatherController : BaseController
{
    private readonly IWeatherService _weatherService;

    public WeatherController(IWeatherService weatherService, ILogger<WeatherController> logger) : base(logger)
    {
        _weatherService = weatherService;
    }
    // This is the GET route for the Location page.
    [HttpGet]
    public IActionResult Location()
    {
        // Show a page asking for the user's permission to use their location.
        return View();
    }
    // This is the POST route for the Location page passing geolocation data.
    [HttpPost]
    public async Task<IActionResult> Weather(decimal latitude, decimal longitude)
    {
        _logger.LogInformation($"Fetching weather for latitude: {latitude}, longitude: {longitude}");

        var weatherResponse = await _weatherService.GetWeather(latitude, longitude);

        if (weatherResponse == null)
        {
            _logger.LogError("Weather response is null");
            Alert("Error Receiving Response, please try again later", AlertType.warning);
            return RedirectToAction(nameof(Location));
        }

        if (weatherResponse.Name.Contains("Londonderry"))
        {
            weatherResponse.Name = "Derry City";
        }

        var weatherViewModel = new WeatherViewModel
        {
            Weather = weatherResponse.Weather[0].Main,
            Icon = weatherResponse.Weather[0].Icon,
            Description = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(weatherResponse.Weather[0].Description),
            Temperature = weatherResponse.Main.Temp,
            Humidity = weatherResponse.Main.Humidity,
            WindSpeed = weatherResponse.Wind.Speed,
            Sunset = DateTimeOffset.FromUnixTimeSeconds(weatherResponse.Sys.Sunset).UtcDateTime.ToString("h:mm tt"),
            Name = weatherResponse.Name,
            Country = weatherResponse.Sys?.Country,
            Advice = _weatherService.GetClothingAdvice(weatherResponse.Main.Temp, weatherResponse.Wind.Speed, weatherResponse.Main.Humidity, weatherResponse.Weather[0].Main)
        };

        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Model state is not valid.");
            Alert("Error Receiving Response", AlertType.warning);
            return RedirectToAction(nameof(Location));
        }
        return View(weatherViewModel);
    }
    // This is the POST route for the Location page passing city name.
    [HttpGet]
    public async Task<IActionResult> WeatherByLocation(string location)
    {
        _logger.LogInformation($"Fetching weather by location: {location}");

        if (string.IsNullOrWhiteSpace(location))
        {
            _logger.LogWarning("Location is empty or null");
            Alert("Please enter a location", AlertType.warning);
            return RedirectToAction(nameof(Location));
        }

        var weatherResponse = await _weatherService.GetWeatherByLocation(location.Replace(" ", ""));
        if (weatherResponse == null)
        {
            Alert("Location not found", AlertType.warning);
            return RedirectToAction(nameof(Location));
        }

        if (weatherResponse.Name == "Londonderry County Borough" || weatherResponse.Name == "Londonderry")
        {
            weatherResponse.Name = "Derry City";
        }

        var weatherViewModel = new WeatherViewModel
        {
            Weather = weatherResponse.Weather[0].Main,
            Icon = weatherResponse.Weather[0].Icon,
            Description = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(weatherResponse.Weather[0].Description),
            Temperature = weatherResponse.Main.Temp,
            Humidity = weatherResponse.Main.Humidity,
            WindSpeed = weatherResponse.Wind.Speed,
            Sunset = DateTimeOffset.FromUnixTimeSeconds(weatherResponse.Sys.Sunset).UtcDateTime.ToString("h:mm tt"),
            Name = weatherResponse.Name,
            Country = weatherResponse.Sys?.Country,
            Advice = _weatherService.GetClothingAdvice(weatherResponse.Main.Temp, weatherResponse.Wind.Speed, weatherResponse.Main.Humidity, weatherResponse.Weather[0].Main)
        };

       if (!ModelState.IsValid)
        {
            _logger.LogError("Weather Model State is not valid");
            foreach (var modelState in ModelState.Values)
            {
                foreach (var error in modelState.Errors)
                {
                    _logger.LogError("Weather Model State Error:" + error.ErrorMessage);
                }
            }
        };
        return View("Weather", weatherViewModel); ;
    }

}

