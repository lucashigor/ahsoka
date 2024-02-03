namespace Ahsoka.Application.Dto.Administrations.Configurations.Requests;

public abstract record BaseConfiguration(string Name,
        string Value,
        string Description,
        DateTime StartDate,
        DateTime ExpireDate);
