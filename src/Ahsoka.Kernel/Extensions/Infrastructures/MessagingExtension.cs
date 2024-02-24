using Ahsoka.Application.Common.Interfaces;
using Ahsoka.Infrastructure.Messaging.RabbitMq;
using Ahsoka.Infrastructure.Repositories.Context;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Ahsoka.Kernel.Extensions.Infrastructures;

public static class MessagingExtension
{
    public static WebApplicationBuilder AddDbMessagingExtension(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IMessageSenderInterface, SendMessagePublisher>();

        builder.Services.AddMassTransit(x =>
        {
            var entryAssembly = AppDomain.CurrentDomain.Load("Ahsoka.Infrastructure");
            x.AddConsumers(entryAssembly);

            x.SetKebabCaseEndpointNameFormatter();
            x.AddEntityFrameworkOutbox<PrincipalContext>(o =>
            {
                o.QueryDelay = TimeSpan.FromSeconds(100);

                o.UsePostgres().UseBusOutbox();
            });

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.AutoStart = true;

                cfg.Host("localhost", "/", h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });

                cfg.ConfigureEndpoints(context);
            });
        });

        return builder;
    }
}
