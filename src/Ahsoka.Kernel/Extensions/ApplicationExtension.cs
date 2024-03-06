using Ahsoka.Application.Administrations.Configurations.Commands.RegisterConfiguration;
using Ahsoka.Application.Administrations.Configurations.Services;
using Ahsoka.Application.Common.Behaviors;
using Ahsoka.Application.Common.Models.Authorizations;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Ahsoka.Kernel.Extensions;

public static class ApplicationExtension
{
    public static WebApplicationBuilder AddApplicationExtensionServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddValidatorsFromAssembly(
                Assembly.GetAssembly(typeof(RegisterConfigurationCommandValidator)));

        builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestValidationBehavior<,>));
        builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingPipelineBehavior<,>));

        builder.Services.AddTransient<IConfigurationServices, ConfigurationServices>();
        builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

        return builder;
    }
}