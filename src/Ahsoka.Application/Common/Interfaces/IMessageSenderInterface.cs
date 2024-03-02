using Ahsoka.Domain.SeedWork;

namespace Ahsoka.Application.Common.Interfaces;

public interface IMessageSenderInterface
{
    Task PubSubSendAsync(object data, CancellationToken cancellationToken);
}
