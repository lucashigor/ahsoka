namespace Ahsoka.Application.Common.Interfaces;

public interface IMessageSenderInterface
{
    Task PubSubSendAsync(object data, CancellationToken cancellationToken);
    Task SendQueueAsync(object data, CancellationToken cancellationToken);
}
