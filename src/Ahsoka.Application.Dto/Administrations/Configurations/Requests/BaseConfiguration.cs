namespace Ahsoka.Application.Dto.Administrations.Configurations.Requests;

public record BaseConfiguration(string Name,
        string Value,
        string Description,
        DateTime StartDate,
        DateTime ExpireDate);
