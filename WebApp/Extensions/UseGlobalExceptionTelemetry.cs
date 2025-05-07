using Microsoft.AspNetCore.Diagnostics;
using System.Diagnostics;

namespace WebApp.Extensions;
public static class GlobalExceptionTelemetryExtension
{
    /// <summary>
    /// Configures global exception handling for capturing detailed exception telemetry
    /// </summary>
    public static IApplicationBuilder UseGlobalExceptionTelemetry(this IApplicationBuilder app)
    {
        app.UseExceptionHandler(errorApp =>
        {
            errorApp.Run(async context =>
            {
                var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                var exception = exceptionHandlerPathFeature?.Error;

                if (exception != null)
                {
                    // Create a new activity to track the exception
                    using var activity = OpenTelemetryServiceExtension.AppActivitySource.StartActivity(
                        "UnhandledException",
                        ActivityKind.Internal);

                    if (activity != null)
                    {
                        // Get the class and method name where the exception occurred
                        var frame = new StackTrace(exception, true).GetFrames()
                            ?.FirstOrDefault(f => f.GetMethod()?.DeclaringType?.Assembly != typeof(object).Assembly);

                        var className = frame?.GetMethod()?.DeclaringType?.FullName ?? "Unknown";
                        var methodName = frame?.GetMethod()?.Name ?? "Unknown";

                        // Add detailed information about the exception
                        //activity.SetStatus(Status.Error.WithDescription(exception.Message));
                        activity.SetTag("exception.type", exception.GetType().FullName);
                        activity.SetTag("exception.message", exception.Message);
                        activity.SetTag("exception.stacktrace", exception.StackTrace);
                        activity.SetTag("exception.source", $"{className}.{methodName}");
                        activity.SetTag("exception.handled", "GlobalHandler");

                        // Add the request path that caused the exception
                        activity.SetTag("request.path", exceptionHandlerPathFeature?.Path);
                    }
                }

                // Return a generic error response
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsync("An unexpected error occurred.");
            });
        });

        return app;
    }

    /// <summary>
    /// Configures global exception handling for capturing detailed exception telemetry on WebApplication
    /// </summary>
    public static WebApplication UseGlobalExceptionTelemetry(this WebApplication app)
    {
        // Reuse the IApplicationBuilder implementation
        ((IApplicationBuilder)app).UseGlobalExceptionTelemetry();
        return app;
    }

}