using Dotnet8.MinimalAPI.Shared.Models;
using Dotnet8.MinimalAPI.Shared.Services;
using Microsoft.Extensions.Logging;

namespace Dotnet8.MinimalAPI.Tests;

public class WeatherServiceTest
{
    private readonly string[] _summaries = ["Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"];

    private readonly ILogger<WeatherForecast> _logger;

    public WeatherServiceTest()
    {
        _logger = Substitute.For<ILogger<WeatherForecast>>();
    }

    [Fact]
    public void GetWeatherForecast_SeperateProject_TypeShouldCorrect()
    {
        WeatherForecast[] result = WeatherService.Forecast3(_summaries, _logger);

        Assert.IsType<WeatherForecast[]>(result);
    }
}