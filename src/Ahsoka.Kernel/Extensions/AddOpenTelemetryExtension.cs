namespace Ahsoka.Kernel.Extensions;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
public static class AddOpenTelemetryExtension
{
    private static readonly string tracingOtlpEndpoint = "";

    public static WebApplicationBuilder AddOpenTelemetry(this WebApplicationBuilder builder)
    {
        var otel = builder.Services.AddOpenTelemetry();

        otel.ConfigureResource(resource => resource.AddService(serviceName: builder.Environment.ApplicationName));

        otel.WithMetrics(metrics => metrics
            .AddAspNetCoreInstrumentation()
            .AddMeter("Microsoft.AspNetCore.Hosting")
            .AddMeter("Microsoft.AspNetCore.Server.Kestrel")
            .AddPrometheusExporter());

        otel.WithTracing(tracing =>
        {
            tracing.AddAspNetCoreInstrumentation();
            tracing.AddHttpClientInstrumentation();
            if (string.IsNullOrEmpty(tracingOtlpEndpoint) is false)
            {
                tracing.AddOtlpExporter(otlpOptions =>
                {
                    otlpOptions.Endpoint = new Uri(tracingOtlpEndpoint);
                });
            }
            else
            {
                tracing.AddConsoleExporter();
            }
        });

        return builder;
    }
}

