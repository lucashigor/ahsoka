using Ahsoka.Application.Common;
using Ahsoka.Application.Common.Interfaces;
using Ahsoka.Infrastructure.Messaging.Consumers.Administrations.Configurations;
using Ahsoka.Infrastructure.Messaging.Publisher.Administrations.Configurations.DomainEventHandlersConfig;
using Ahsoka.Infrastructure.Messaging.RabbitMq;
using Ahsoka.Infrastructure.Repositories.Context;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ahsoka.Kernel.Extensions.Infrastructures;

public static class MessagingExtension
{
    public static WebApplicationBuilder AddDbMessagingExtension(this WebApplicationBuilder builder)
    {
        var configs = builder.Configuration
            .GetSection(nameof(RabbitMq))
            .Get<RabbitMq>();

        if (configs == null)
        {
            return builder;
        }

        builder.Services.AddScoped<IMessageSenderInterface, SendMessagePublisher>();

        builder.Services.AddMassTransit(x =>
        {
            x.SetKebabCaseEndpointNameFormatter();
            x.AddEntityFrameworkOutbox<PrincipalContext>(o =>
            {
                o.QueryDelay = TimeSpan.FromSeconds(200);

                o.UsePostgres().UseBusOutbox();
            });

            x.AddConfigurationsConsumers();

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.AutoStart = true;

                cfg.Host(configs.Host!, "/", h =>
                {
                    h.Username(configs.Username!);
                    h.Password(configs.Password!);
                });

                cfg
                .AddConfigurationsPublisherDomainEventHandlersConfigs()
                .AddConfigurationsConsumerDomainEventHandlersConfigs(context);
            });
        });

        return builder;
    }
}
