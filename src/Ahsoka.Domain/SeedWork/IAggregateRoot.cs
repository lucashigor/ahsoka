namespace Ahsoka.Domain;

public interface IAggregateRoot
{
    IReadOnlyCollection<DomainEventBase> Events { get; }

    void ClearEvents();
}
