using Azure.Monitor.OpenTelemetry.AspNetCore;

namespace WebApp.Extensions;
public static class OpenTelemetryServiceExtension
{
    public static IServiceCollection AddOpenTelemetryExtension(this IServiceCollection services, WebApplicationBuilder builder)
    {
        services.AddOpenTelemetry().UseAzureMonitor(options =>
        {
            options.ConnectionString = Environment.GetEnvironmentVariable("AppInsights", EnvironmentVariableTarget.Process) ?? builder.Configuration["OpenTelemetry:ConnectionString"];
        });
        //services.AddOpenTelemetry()
        //    .WithTracing(builder =>
        //    {
        //        builder
        //            .AddAspNetCoreInstrumentation()
        //            .AddHttpClientInstrumentation()
        //            .AddEntityFrameworkCoreInstrumentation()
        //            .AddSource("AppGlobal")
        //            .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("WebApp"))
        //            .AddOtlpExporter(options =>
        //            {
        //                options.Endpoint = new Uri(configuration["OpenTelemetry:OtlpEndpoint"]);
        //                options.Protocol = OtlpExportProtocol.Grpc;
        //            });
        //    });
        return services;
    }
}
