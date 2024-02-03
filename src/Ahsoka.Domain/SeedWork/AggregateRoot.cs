using System.Collections.Immutable;

namespace Ahsoka.Domain.SeedWork;

public abstract class AggregateRoot<T> : Entity<T>, IAggregateRoot where T : IEquatable<T>
{
    protected AggregateRoot()
    {
        _events = new HashSet<DomainEventBase>();
    }


    private readonly ICollection<DomainEventBase> _events;

    public IReadOnlyCollection<DomainEventBase> Events => _events.ToImmutableArray();

    public void ClearEvents()
    {
        _events.Clear();
    }

    internal void RaiseDomainEvent(DomainEventBase @event)
    {
        _events.Add(@event);
    }
}
