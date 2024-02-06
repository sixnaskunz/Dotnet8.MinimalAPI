
namespace Dotnet8.MinimalAPI.Tests;

public class GlobalExceptionHandlerTests
{
    [Fact]
    public async Task TryHandleAsync_ShouldSetResponseStatusCodeAndWriteProblemDetailsAsJson()
    {
        IConfigurationRoot configuration = new ConfigurationBuilder().AddJsonFile("appsettings.debug.json", false, true).Build();
        HttpContext httpContext = Substitute.For<HttpContext>();
        HttpResponse httpResponse = Substitute.For<HttpResponse>();
        IExceptionHandlerFeature exceptionHandlerFeature = Substitute.For<IExceptionHandlerFeature>();
        MemoryStream memoryStream = new();
        StreamWriter streamWriter = new(memoryStream);

        Activity activity = Substitute.For<Activity>("TestActivity");
        activity.Start();
        Activity.Current = activity;
        string? activityId = Activity.Current?.Id;
        httpResponse.Body.Returns(streamWriter.BaseStream);
        httpContext.Response.Returns(httpResponse);
        httpContext.Features.Get<IExceptionHandlerFeature>().Returns(exceptionHandlerFeature);
        httpContext.TraceIdentifier.Returns("0HLQ5C0F5PO7M:00000001");

        GlobalExceptionHandler handler = new(configuration);

        await handler.TryHandleAsync(httpContext, new Exception("Test exception"), CancellationToken.None);
        memoryStream.Seek(0, SeekOrigin.Begin);

        string responseBody = await new StreamReader(memoryStream).ReadToEndAsync();
        JsonDocument jsonDocument = JsonDocument.Parse(responseBody);

        Microsoft.AspNetCore.Mvc.ProblemDetails? problemDetails = JsonSerializer.Deserialize<Microsoft.AspNetCore.Mvc.ProblemDetails>(responseBody);

        Assert.NotNull(problemDetails);

        // Extract properties from the JSON document
        problemDetails.Status = jsonDocument.RootElement.GetProperty("status").GetInt32();
        problemDetails.Title = jsonDocument.RootElement.GetProperty("title").GetString();
        problemDetails.Detail = jsonDocument.RootElement.GetProperty("detail").GetString();
        problemDetails.Extensions.TryGetValue("traceId", out object? traceId);

        Assert.Equal(StatusCodes.Status500InternalServerError, problemDetails.Status);
        Assert.Equal("Internal Server Error", problemDetails.Title);
        Assert.Equal("Test exception", problemDetails.Detail);
        Assert.NotNull(traceId);
        Assert.Equal(activityId, traceId.ToString());
    }
}