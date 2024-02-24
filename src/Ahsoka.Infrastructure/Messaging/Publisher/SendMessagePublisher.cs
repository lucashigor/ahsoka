using Ahsoka.Application.Common.Interfaces;
using MassTransit;

namespace Ahsoka.Infrastructure.Messaging.RabbitMq;

public class SendMessagePublisher(IPublishEndpoint bus) : IMessageSenderInterface
{
    private readonly IPublishEndpoint _bus = bus;

    public async Task PubSubSendAsync(object data, CancellationToken cancellationToken)
    {
        await _bus.Publish(data, cancellationToken);
    }

    public async Task SendQueueAsync(object data, CancellationToken cancellationToken)
    {
        await _bus.Publish(data, cancellationToken);
    }
}
