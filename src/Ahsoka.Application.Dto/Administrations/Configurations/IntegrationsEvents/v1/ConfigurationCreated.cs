namespace Ahsoka.Application.Dto.Administrations.Configurations.IntegrationsEvents.v1;

public class ConfigurationCreated
{
    public string CustomerType { get; init; }
    public Guid TransactionId { get; init; }
    public Guid Id { get; init; }
    public string Name { get; init; }
    public string Value { get; init; }
    public string Description { get; init; }
    public DateTime StartDate { get; init; }
    public DateTime? ExpireDate { get; init; }

}
