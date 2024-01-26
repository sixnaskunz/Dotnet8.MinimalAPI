using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Dotnet8.MinimalAPI;

/// <summary>
/// Represents a global exception handler for handling unhandled exceptions in the application.
/// Docs: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/error-handling?view=aspnetcore-8.0#iexceptionhandler
/// </summary>
internal sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger, IConfiguration configuration) : IExceptionHandler
{
    /// <summary>
    /// Tries to handle the specified exception asynchronously.
    /// </summary>
    /// <param name="httpContext">The current HTTP context.</param>
    /// <param name="exception">The exception to handle.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="ValueTask{TResult}"/> representing the asynchronous operation, where the result indicates whether the exception was handled successfully.</returns>
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        logger.LogError(exception, "Exception occurred: {Message}", exception.Message);

        IExceptionHandlerFeature? handlerFeature = httpContext.Features.Get<IExceptionHandlerFeature>();
        bool enableDebugMessage = configuration.GetValue<bool>("Exception:Debug");

        ProblemDetails problemDetails = new()
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "Internal Server Error",
            Extensions = new Dictionary<string, object?>
            {
                { "traceId",  httpContext.TraceIdentifier }
            },
            Instance = handlerFeature?.Path,
            Detail = enableDebugMessage ? exception.Message : null
        };
        problemDetails.Extensions.Add("errors", new { payload = "ทดสอบ" });

        httpContext.Response.StatusCode = problemDetails.Status.Value;

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}