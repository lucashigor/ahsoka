namespace Ahsoka.Application.Administrations.Configurations.Commands;

public abstract record BaseConfiguration(string Name,
    string Value,
    string Description,
    DateTime StartDate,
    DateTime? FinalDate);
