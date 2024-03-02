namespace Ahsoka.Domain.SeedWork;

public abstract record DomainEventBase(string EventName)
{
    public DateTime EventDateUTC { get; init; } = DateTime.UtcNow;
}

