using Ahsoka.Domain.Common.ValuesObjects;
using Ahsoka.Domain.Validation;
using System.Collections.Immutable;

namespace Ahsoka.Domain.SeedWork;

public abstract class Entity<T> where T : IEquatable<T>
{
    public T Id { get; init; }
    protected readonly ICollection<Notification> _notifications;
    protected IReadOnlyCollection<Notification> Notifications => _notifications.ToImmutableArray();

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    protected Entity()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    {
        _notifications = new HashSet<Notification>();
    }

    protected virtual Result Validate()
    {
        AddNotification(Id!.NotNull());

        if (Notifications.Count != 0)
        {
            return Result.Failure(_notifications);
        }

        return Result.Success();
    }

    protected void AddNotification(Notification? notification)
    {
        if (notification != null)
        {
            _notifications.Add(notification);
        }
    }

    protected void AddNotification(string message, DomainErrorCode domainError)
        => AddNotification(new(message, domainError));

    protected void AddNotification(string fieldName, string message, DomainErrorCode domainError)
        => AddNotification(new(fieldName, message, domainError));

}
