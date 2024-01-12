using Dotnet8.MinimalAPI;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ConfigureHttpJsonOptions Docs: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/responses?view=aspnetcore-8.0#configure-json-serialization-options-globally
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.WriteIndented = true;
    options.SerializerOptions.IncludeFields = true;
    options.SerializerOptions.PropertyNameCaseInsensitive = true;
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
});

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

string[] summaries =
[
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
];

RouteGroupBuilder weatherGroup = app.MapGroup("/weather");
weatherGroup.MapGet("/forecast", Forecast(summaries));
//? มีปัญหา ILogger ไม่สามารถ pass static class เข้าไปใน TGeneric ได้
weatherGroup.MapGet("/forecast2", (ILogger<WeatherForecast> logger) => Forecast2(summaries, logger));
weatherGroup.MapGet("/forecast3", (ILogger<WeatherForecast> logger) => WeatherService.Forecast3(summaries, logger));
weatherGroup.MapGet("/forecast-internal-assembly", (ILogger<InternalWeatherForecast> logger) => InternalWeatherService.InternalAssemblyForecast(summaries, logger));

app.Run();

static Func<WeatherForecast[]> Forecast(string[] summaries)
{
    return () =>
    {
        WeatherForecast[] forecast = Enumerable.Range(1, 5).Select(index =>
            new WeatherForecast
            (
                DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                Random.Shared.Next(-20, 55),
                summaries[Random.Shared.Next(summaries.Length)]
            ))
            .ToArray();
        return forecast;
    };
}

static WeatherForecast[] Forecast2(string[] summaries, ILogger<WeatherForecast> logger)
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