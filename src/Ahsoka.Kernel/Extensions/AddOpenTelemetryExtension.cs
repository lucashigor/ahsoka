namespace Ahsoka.Kernel.Extensions;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Npgsql;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

public static class AddOpenTelemetryExtension
{
    public static WebApplicationBuilder AddOpenTelemetry(this WebApplicationBuilder builder)
    {
        Action<ResourceBuilder> configureResource = r => r.AddService(
        serviceName: "ahsoka.api",
        serviceVersion: "1.0",
        serviceInstanceId: Environment.MachineName);

        builder.Services.AddOpenTelemetry()
        .ConfigureResource(configureResource)
        .WithTracing(tracing =>
        {
            tracing.AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddNpgsql()
            .AddOtlpExporter(opt =>
            {
                opt.Endpoint = new Uri("http://localhost:4318");
                opt.Protocol = OtlpExportProtocol.Grpc;
            });
        })
        .WithMetrics(metrics => metrics
            .AddAspNetCoreInstrumentation()
            .AddMeter("Microsoft.AspNetCore.Hosting")
            .AddMeter("Microsoft.AspNetCore.Server.Kestrel")
            .AddOtlpExporter(opt =>
            {
                opt.Endpoint = new Uri("http://localhost:4318");
                opt.Protocol = OtlpExportProtocol.Grpc;
            }));

        builder.Logging.ClearProviders();

        builder.Logging
        .AddOpenTelemetry(options =>
        {
            options.AddOtlpExporter(opt =>
            {
                opt.Endpoint = new Uri("http://localhost:4318");
                opt.Protocol = OtlpExportProtocol.Grpc;
            });

            options.AddConsoleExporter();
        });


        return builder;
    }
}

