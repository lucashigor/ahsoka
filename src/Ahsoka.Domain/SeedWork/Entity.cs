using System.Collections.Immutable;

namespace Ahsoka.Domain;

public abstract class Entity
{
    public Guid Id { get; init; }
    private readonly ICollection<Notification> _notifications;
    public IReadOnlyCollection<Notification> Notifications => _notifications.ToImmutableArray();

    protected Entity()
    {
        _notifications = new HashSet<Notification>();
    }

    protected virtual void Validate()
    {
        AddNotification(Id!.NotNull());

        if (Notifications.Any())
        {
            throw new InvalidDomainException(Notifications.ToList().GetMessage(), CommonErrorCodes.Validation);
        }
    }

    protected void AddNotification(Notification notification)
    {
        if (notification != null)
        {
            _notifications.Add(notification);
        }
    }

}
