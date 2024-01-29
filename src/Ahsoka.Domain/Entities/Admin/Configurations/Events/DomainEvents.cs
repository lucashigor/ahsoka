namespace Ahsoka.Domain;
public record ConfigurationCreatedDomainEvent : DomainEventBase
{
	public ConfigurationCreatedDomainEvent(Configuration configuration) : base(nameof(ConfigurationCreatedDomainEvent))
	{
        Configuration = configuration;
    }

    public Configuration Configuration { get; set; }
}
