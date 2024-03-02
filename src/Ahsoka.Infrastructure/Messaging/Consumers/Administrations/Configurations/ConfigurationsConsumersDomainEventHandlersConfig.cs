using Ahsoka.Domain.Entities.Admin.Configurations.Events;
using Ahsoka.Infrastructure.Messaging.Consumers.Administrations.Configurations.DomainEventHandlers;
using Ahsoka.Infrastructure.Messaging.Publisher;
using MassTransit;
using RabbitMQ.Client;

namespace Ahsoka.Infrastructure.Messaging.Consumers.Administrations.Configurations;

public static class ConfigurationsConsumersDomainEventHandlersConfig
{
    public static IBusRegistrationConfigurator AddConfigurationsConsumers(this IBusRegistrationConfigurator config)
    {
        config.AddConsumer<ConfigurationCreatedDomainEventConsumer>();

        return config;
    }

    public static IRabbitMqBusFactoryConfigurator AddConfigurationsConsumerDomainEventHandlersConfigs(this IRabbitMqBusFactoryConfigurator config, IRegistrationContext context)
    {
        config.ReceiveEndpoint($"{nameof(ConfigurationCreatedDomainEventConsumer)}-queues", x =>
        {
            x.ConfigureConsumeTopology = false;

            x.Consumer<ConfigurationCreatedDomainEventConsumer>(context);

            x.Bind(TopicNames.CONFIGURATION_DOMAIN_TOPIC, s =>
            {
                s.RoutingKey = nameof(ConfigurationCreatedDomainEvent);
                s.ExchangeType = ExchangeType.Direct;
            });
        });

        return config;
    }
}
