namespace Ahsoka.Infrastructure.Messaging.Publisher.Administrations.Configurations.DomainEventHandlersConfig;

using Ahsoka.Domain.Entities.Admin.Configurations.Events;
using MassTransit;
using RabbitMQ.Client;

public static class ConfigurationsPublisherDomainEventHandlersConfig
{
    public static IRabbitMqBusFactoryConfigurator AddConfigurationsPublisherDomainEventHandlersConfigs(this IRabbitMqBusFactoryConfigurator config)
    {
        config.Send<ConfigurationCreatedDomainEvent>(x =>
        {
            x.UseRoutingKeyFormatter(context => context.Message.EventName);
            x.UseCorrelationId(context => context.Id);
        });

        config.Message<ConfigurationCreatedDomainEvent>(x => x.SetEntityName(TopicNames.CONFIGURATION_DOMAIN_TOPIC));
        config.Publish<ConfigurationCreatedDomainEvent>(x => x.ExchangeType = ExchangeType.Direct);

        config.Send<ConfigurationDeletedDomainEvent>(x =>
        {
            x.UseRoutingKeyFormatter(context => context.Message.EventName);
            x.UseCorrelationId(context => context.Id);
        });

        config.Message<ConfigurationDeletedDomainEvent>(x => x.SetEntityName(TopicNames.CONFIGURATION_DOMAIN_TOPIC));
        config.Publish<ConfigurationDeletedDomainEvent>(x => x.ExchangeType = ExchangeType.Direct);

        return config;
    }
}


