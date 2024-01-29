namespace Ahsoka.Application.Dto;

public record ConfigurationOutput(Guid Id, 
        string Name, 
        string Value, 
        string Description, 
        DateTimeOffset StartDate, 
        DateTimeOffset? FinalDate);
