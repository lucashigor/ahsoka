using Ahsoka.Domain.Common.ValuesObjects;
using System.Collections.Immutable;

namespace Ahsoka.Domain.Exceptions;

public sealed class InvalidDomainException(string? message, ICollection<Notification> notifications) : Exception(message)
{
    private readonly ICollection<Notification> _notifications = notifications;

    public IReadOnlyCollection<Notification> Notifications => _notifications.ToImmutableArray();
}
