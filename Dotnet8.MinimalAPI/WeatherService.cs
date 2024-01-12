namespace Dotnet8.MinimalAPI;

internal static class WeatherService
{
    internal static WeatherForecast[] Forecast3(string[] summaries, ILogger<WeatherForecast> logger)
    {
        WeatherForecast[] forecast = Enumerable.Range(1, 5).Select(index =>
               new WeatherForecast
               (
                   DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                   Random.Shared.Next(-20, 55),
                   summaries[Random.Shared.Next(summaries.Length)]
               ))
               .ToArray();
        logger.LogInformation("LogInfo: {forcast}", forecast);
        return forecast;
    }
}