namespace Ahsoka.Application.Dto.Administrations.Configurations.Requests;

public record RegisterConfigurationInput(string Name, string Value, string Description, DateTime StartDate, DateTime ExpireDate)
    : BaseConfiguration(Name, Value, Description, StartDate, ExpireDate);
