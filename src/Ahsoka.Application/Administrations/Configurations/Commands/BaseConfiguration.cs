namespace Ahsoka.Application.Administrations.Configurations.Commands;
public record BaseConfiguration(string Name, string Value, string Description, DateTime StartDate, DateTime? ExpireDate)
{
    public BaseConfiguration(Dto.Administrations.Configurations.Requests.BaseConfiguration baseConfiguration) : this(
        baseConfiguration.Name,
        baseConfiguration.Value,
        baseConfiguration.Description,
        baseConfiguration.StartDate,
        baseConfiguration.ExpireDate)
    {
    }
}
