namespace Ahsoka.Application.Dto.Administrations.Configurations.Responses;

public record ConfigurationOutput(Guid Id,
        string Name,
        string Value,
        string Description,
        DateTime StartDate,
        DateTime? ExpireDate);
