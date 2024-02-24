using Ahsoka.Application.Common.Interfaces;
using Ahsoka.Domain.Entities.Admin.Configurations.Repository;
using Ahsoka.Infrastructure;
using Ahsoka.Infrastructure.Repositories.Common;
using Ahsoka.Infrastructure.Repositories.Configurations;
using Ahsoka.Infrastructure.Repositories.Context;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ahsoka.Kernel.Extensions.Infrastructures;

public static class DbExtension
{
    public static WebApplicationBuilder AddDbExtension(this WebApplicationBuilder builder)
    {
        var conn = builder.Configuration.GetConnectionString("PrincipalDatabase");

        if (string.IsNullOrEmpty(conn) is false)
        {
            builder.Services.AddDbContext<PrincipalContext>(options =>
            {
                options.EnableDetailedErrors();
                options.EnableSensitiveDataLogging();

                options.UseNpgsql(conn, x =>
                {
                    x.EnableRetryOnFailure(5);
                    x.MinBatchSize(1);
                });
            });

            var serviceProvider = builder.Services.BuildServiceProvider();
            using var scope = serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<PrincipalContext>();
            db.Database.Migrate();
        }

        builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(TransactionBehavior<,>));

        builder.Services.AddScoped<ICommandsConfigurationRepository, CommandsConfigurationRepository>();
        builder.Services.AddScoped<IQueriesConfigurationRepository, QueriesConfigurationRepository>();
        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

        return builder;
    }
}
