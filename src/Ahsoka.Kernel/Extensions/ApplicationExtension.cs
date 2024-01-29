using System.Reflection;
using Ahsoka.Application;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Ahsoka.Kernel;

public static class ApplicationExtension
{
    public static WebApplicationBuilder AddApplicationExtensionServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<Notifier>();

        builder.Services.AddValidatorsFromAssembly(
                Assembly.GetAssembly(typeof(RegisterConfigurationCommandValidator)));

        builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestValidationBehavior<,>));
        builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestExceptionBehavior<,>));
        builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingPipelineBehavior<,>));

        builder.Services.AddTransient<IDateValidationServices, DateValidationServices>();
        builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

        return builder;
    }
}