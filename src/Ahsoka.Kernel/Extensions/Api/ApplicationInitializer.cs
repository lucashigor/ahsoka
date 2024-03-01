using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Reflection;

namespace Ahsoka.Kernel.Extensions.Api;

public static class ApplicationInitializer
{
    private static readonly Assembly DomainAssembly;
    private static readonly Assembly InfrastructureAssembly;
    private static readonly Assembly ApplicationAssembly;

    static ApplicationInitializer()
    {
        DomainAssembly = AppDomain.CurrentDomain.Load("Ahsoka.Domain");
        ApplicationAssembly = AppDomain.CurrentDomain.Load("Ahsoka.Application");
        InfrastructureAssembly = AppDomain.CurrentDomain.Load("Ahsoka.Infrastructure");
    }

    public static WebApplicationBuilder AddApiExtensionServices(this WebApplicationBuilder builder)
    {
        var assembly1 = Assembly.GetExecutingAssembly();

        builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly1));
        builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(DomainAssembly));
        builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(InfrastructureAssembly));
        builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(ApplicationAssembly));

        builder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

        builder.Services.AddHealthChecks();

        return builder;
    }
}
