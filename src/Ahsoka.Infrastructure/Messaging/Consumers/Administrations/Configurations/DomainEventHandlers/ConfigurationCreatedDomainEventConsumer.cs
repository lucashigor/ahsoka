using Ahsoka.Application.Administrations.Configurations.DomainEventHandlers;
using Ahsoka.Domain.Entities.Admin.Configurations.Events;
using MassTransit;
using MediatR;

namespace Ahsoka.Infrastructure.Messaging.Consumers.Administrations.Configurations.DomainEventHandlers;

public class ConfigurationCreatedDomainEventConsumer(IMediator mediator) : IConsumer<ConfigurationCreatedDomainEvent>
{
    private readonly IMediator _mediator = mediator;
    public async Task Consume(ConsumeContext<ConfigurationCreatedDomainEvent> context)
    {
        await _mediator.Send(new ConfigurationCreatedDomainEventCommand(context.Message));
    }
}
