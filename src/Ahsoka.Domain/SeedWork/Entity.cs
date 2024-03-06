using Ahsoka.Domain.Common.ValuesObjects;
using Ahsoka.Domain.Validation;
using System.Collections.Immutable;

namespace Ahsoka.Domain.SeedWork;

public abstract class Entity<TEntityId> where TEntityId : IEquatable<TEntityId>
{
    public TEntityId Id { get; init; }
    protected readonly ICollection<Notification> _notifications;

    protected IReadOnlyCollection<Notification> Notifications => _notifications.ToImmutableArray();
    protected readonly ICollection<Notification> _warnings;
    
    protected IReadOnlyCollection<Notification> Warnings => _warnings.ToImmutableArray();

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    protected Entity()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    {
        _notifications = new HashSet<Notification>();
        _warnings = new HashSet<Notification>();
    }

    protected virtual DomainResult Validate()
    {
        AddNotification(Id!.NotNull());

        if (Notifications.Count != 0)
        {
            return DomainResult.Failure(errors: _notifications);
        }

        return DomainResult.Success(warnings: _warnings);
    }

    protected void AddNotification(Notification? notification)
    {
        if (notification != null)
        {
            _notifications.Add(notification);
        }
    }

    protected void AddNotification(string fieldName, string message, DomainErrorCode domainError)
        => AddNotification(new(fieldName, message, domainError));

    protected void AddWarning(Notification? notification)
    {
        if (notification != null)
        {
            _warnings.Add(notification);
        }
    }

    protected void AddWarning(string fieldName, string message, DomainErrorCode domainError)
        => AddWarning(new(fieldName, message, domainError));

}
