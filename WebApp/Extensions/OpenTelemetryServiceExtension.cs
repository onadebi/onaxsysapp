using Azure.Identity;
using Azure.Monitor.OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System.Diagnostics;
using System.Reflection;

namespace WebApp.Extensions;
public static class OpenTelemetryServiceExtension
{
    // ActivitySource for manual instrumentation
    public static readonly ActivitySource AppActivitySource = new("AppGlobal");
    public static IServiceCollection AddOpenTelemetryExtension(this IServiceCollection services, WebApplicationBuilder builder)
    {
        string AzKeyVaultClientSecret = Environment.GetEnvironmentVariable("AzKeyVaultClientSecret", EnvironmentVariableTarget.Process)!;
        string AzKeyVaultKeyVaultUrl = Environment.GetEnvironmentVariable("AzKeyVaultKeyVaultUrl", EnvironmentVariableTarget.Process)!;

        builder.Configuration.AddAzureKeyVault(
            new Uri(AzKeyVaultKeyVaultUrl),
            new ClientSecretCredential(
                builder.Configuration.GetValue<string>("AppSettings:AzKeyVault:VaultDirectoryTenantId")!,
                builder.Configuration.GetValue<string>("AppSettings:AzKeyVault:ApplicationClientId")!,
                AzKeyVaultClientSecret
            )
        );

        var config = builder.Configuration;
        var serviceName = config.GetValue<string>("OpenTelemetry:ServiceName") ?? "onaxapp-base";
        var serviceVersion = config.GetValue<string>("OpenTelemetry:ServiceVersion") ?? Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "1.0.0";
        var connectionString = Environment.GetEnvironmentVariable("AppInsights", EnvironmentVariableTarget.Process);
        if(string.IsNullOrWhiteSpace(connectionString))
        {
            connectionString = config.GetValue<string>("OpenTelemetry:ConnectionString");
        } 

        // Add OpenTelemetry services
        services.AddOpenTelemetry()
            .WithTracing(tracerProviderBuilder =>
            {
                tracerProviderBuilder
                    .AddSource(AppActivitySource.Name)
                    .SetResourceBuilder(
                        ResourceBuilder.CreateDefault()
                            .AddService(serviceName, serviceVersion: serviceVersion)
                            .AddAttributes(new Dictionary<string, object>
                            {
                                ["deployment.environment"] = builder.Environment.EnvironmentName
                            })
                    )
                    .AddAspNetCoreInstrumentation(options =>
                    {
                        options.RecordException = true;
                        // Capture request headers and response codes
                        options.EnrichWithHttpRequest = (activity, httpRequest) =>
                        {
                            activity.DisplayName = httpRequest.Path;
                            activity.SetTag("http.request.host", httpRequest.Host.Value);
                            activity.SetTag("http.request.path", httpRequest.Path.Value);
                        };
                        options.EnrichWithException = (activity, exception) =>
                        {
                            activity.DisplayName = exception.Message;
                            activity.SetTag("exception.type", exception.GetType().FullName);
                            activity.SetTag("exception.message", exception.Message);
                            activity.SetTag("exception.stacktrace", exception.StackTrace);
                        };
                        options.Filter = (HttpContext context) =>
                        {
                            // Filter out health check and login requests
                            return !context.Request.Path.StartsWithSegments("/health") &&
                                   !context.Request.Path.StartsWithSegments("/Account/Login");
                        };
                    })
                    .AddHttpClientInstrumentation()
                    .AddSqlClientInstrumentation(options =>
                    {
                        options.RecordException = true;
                        options.SetDbStatementForText = true;
                    });

                // Check if EntityFrameworkCore instrumentation package is available
                try
                {
                    tracerProviderBuilder.AddEntityFrameworkCoreInstrumentation(options =>
                    {
                        options.SetDbStatementForText = true;
                    });
                }
                catch (Exception ex)
                {
                    // Log the error or handle it silently if EF Core package is not available
                    Console.WriteLine($"Error adding EF Core instrumentation: {ex.Message}");
                }

                // Add Azure Monitor exporter
                tracerProviderBuilder.AddAzureMonitorTraceExporter(options =>
                {
                    options.ConnectionString = connectionString;
                });
            })
            .WithMetrics(metricsProviderBuilder =>
            {
                metricsProviderBuilder
                    .SetResourceBuilder(
                        ResourceBuilder.CreateDefault()
                        .AddService(serviceName, serviceVersion: serviceVersion)
                    )
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation()
                    .AddAzureMonitorMetricExporter(options =>
                    {
                        options.ConnectionString = connectionString;
                    });
            });

        return services;
    }
}
