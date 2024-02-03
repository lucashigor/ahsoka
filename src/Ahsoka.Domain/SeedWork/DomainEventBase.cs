namespace Ahsoka.Domain.SeedWork;

public abstract record DomainEventBase
{
    public Guid EventId { get; }
    public DateTime EventDateUtc { get; }
    public string EventName { get; set; }

    public DomainEventBase(string eventName)
    {
        EventId = Guid.NewGuid();
        EventDateUtc = DateTime.UtcNow;
        EventName = eventName;
    }
}
