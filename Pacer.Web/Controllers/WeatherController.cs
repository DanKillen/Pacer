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

    [HttpGet]
    public IActionResult Location()
    {
        // Show a page asking for the user's permission to use their location.
        return View();
    }
    
    [HttpPost]
    public async Task<IActionResult> Weather(decimal latitude, decimal longitude)
    {
        var weatherResponse = await _weatherService.GetWeather(latitude, longitude);
        Console.WriteLine(JsonSerializer.Serialize(weatherResponse));

        if (weatherResponse.Name == "Londonderry County Borough")
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
            Alert("Error Receiving Response", AlertType.warning);
            return RedirectToAction(nameof(Location));
        }
        return View(weatherViewModel);
    }

    [HttpGet]
    public async Task<IActionResult> WeatherByLocation(string location)
    {

        var weatherResponse = await _weatherService.GetWeatherByLocation(location.Replace(" ", ""));
        if (weatherResponse == null)
        {
            Alert("Location not found", AlertType.warning);
            return RedirectToAction(nameof(Location));
        }
        Console.WriteLine(JsonSerializer.Serialize(weatherResponse));

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

