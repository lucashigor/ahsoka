using Ahsoka.Infrastructure.Repositories.Context;
using Google.Protobuf.WellKnownTypes;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Ahsoka.Kernel.Extensions.Api;

public static class HealthChecksExtension
{
    public static WebApplicationBuilder ConfigureHealthChecks(this WebApplicationBuilder builder)
    {
        var conn = builder.Configuration.GetConnectionString(nameof(PrincipalContext));

        builder.Services.AddHealthChecks();

        builder.Services.AddHealthChecks()
            .AddNpgSql(conn, healthQuery: "select 1", name: "Postgres", failureStatus: HealthStatus.Unhealthy, tags: new[] { "Database" });

        builder.Services.AddHealthChecksUI(opt =>
        {
            opt.SetEvaluationTimeInSeconds(10); 
            opt.MaximumHistoryEntriesPerEndpoint(60);
            opt.SetApiMaxActiveRequests(1);
            opt.AddHealthCheckEndpoint("Ahsoka api", "/health");

        })
            .AddInMemoryStorage();
        return builder;
    }

    public static WebApplication ConfigureHealthChecks(this WebApplication app)
    {

        app.MapHealthChecks("/health", new HealthCheckOptions
        {
            Predicate = _ => true,
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        }).WithMetadata(new AllowAnonymousAttribute());

        app.UseHealthChecksUI(options =>
        {
            options.UIPath = "/healthcheck-ui";
            //options.AddCustomStylesheet("./HealthCheck/Custom.css");
        });

        return app;
    }

}