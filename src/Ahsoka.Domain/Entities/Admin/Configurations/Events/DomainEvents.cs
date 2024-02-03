using Ahsoka.Domain.SeedWork;

namespace Ahsoka.Domain.Entities.Admin.Configurations.Events;
public record ConfigurationCreatedDomainEvent(Configuration Configuration) :
    DomainEventBase(nameof(ConfigurationCreatedDomainEvent));

public record ConfigurationUpdatedDomainEvent(Configuration Configuration) :
    DomainEventBase(nameof(ConfigurationUpdatedDomainEvent));

public record ConfigurationDeletedDomainEvent(Configuration Configuration) :
    DomainEventBase(nameof(ConfigurationDeletedDomainEvent));
