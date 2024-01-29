using Ahsoka.Application;
using Ahsoka.Domain;
using Ahsoka.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ahsoka.Kernel;

public static class InfrastructureExtension
{
    public static WebApplicationBuilder AddInfrastructureServices(this WebApplicationBuilder builder)
    {
        var conn = builder.Configuration.GetConnectionString("PrincipalDatabase");

        if(string.IsNullOrEmpty(conn) is false)
        {
            builder.Services.AddDbContext<PrincipalContext>(options =>
            {
                options.EnableDetailedErrors();
                options.EnableSensitiveDataLogging();
                options.UseNpgsql(conn!);
            });

        var serviceProvider = builder.Services.BuildServiceProvider();
        using var scope = serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PrincipalContext>();
        db.Database.Migrate();
        
        }
        
        builder.Services.AddScoped<IConfigurationRepository, ConfigurationRepository>();
        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

        builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(TransactionBehavior<,>));

        return builder;
    }
}
