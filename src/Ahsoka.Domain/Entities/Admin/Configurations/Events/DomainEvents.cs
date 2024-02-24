using Ahsoka.Domain.SeedWork;

namespace Ahsoka.Domain.Entities.Admin.Configurations.Events;
public record ConfigurationCreatedDomainEvent : DomainEventBase
{
    public ConfigurationCreatedDomainEvent() : base(nameof(ConfigurationCreatedDomainEvent))
    {
    }

    public ConfigurationCreatedDomainEvent(Configuration Configuration) : base(nameof(ConfigurationCreatedDomainEvent))
    {
        Id = Configuration.Id;
        Name = Configuration.Name;
        Value = Configuration.Value;
        Description = Configuration.Description;
        StartDate = Configuration.StartDate;
        ExpireDate = Configuration.ExpireDate;
        CreatedBy = Configuration.CreatedBy;
        CreatedAt = Configuration.CreatedAt;
        IsDeleted = Configuration.IsDeleted;
    }
    public Guid Id { get; init; }
    public string Name { get; init; }
    public string Value { get; init; }
    public string Description { get; init; }
    public DateTime StartDate { get; init; }
    public DateTime? ExpireDate { get; init; }
    public string CreatedBy { get; init; }
    public DateTime CreatedAt { get; init; }
    public bool IsDeleted { get; init; }

}

public record ConfigurationUpdatedDomainEvent(Configuration Configuration) :
    DomainEventBase(nameof(ConfigurationUpdatedDomainEvent));

public record ConfigurationDeletedDomainEvent(Configuration Configuration) :
    DomainEventBase(nameof(ConfigurationDeletedDomainEvent));
