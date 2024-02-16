namespace Ahsoka.Domain.SeedWork;

public abstract record DomainEventBase
{
    public Guid EventId { get; }
    public DateTime EventDateUTC { get; }
    public string EventName { get; set; }

    public DomainEventBase(string eventName)
    {
        EventId = Guid.NewGuid();
        EventDateUTC = DateTime.UtcNow;
        EventName = eventName;
    }
}
