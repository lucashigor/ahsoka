namespace Ahsoka.Application;

public abstract record BaseConfiguration(string Name,
    string Value,
    string Description,
    DateTimeOffset StartDate,
    DateTimeOffset? FinalDate);
