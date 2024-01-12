namespace Dotnet8.MinimalAPI;

internal static class InternalWeatherService
{
    internal static InternalWeatherForecast[] InternalAssemblyForecast(string[] summaries, ILogger<InternalWeatherForecast> logger)
    {
        InternalWeatherForecast[] forecast = Enumerable.Range(1, 5).Select(index =>
               new InternalWeatherForecast
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