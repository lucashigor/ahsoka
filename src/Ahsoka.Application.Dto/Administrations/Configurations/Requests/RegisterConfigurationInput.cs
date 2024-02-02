
namespace Ahsoka.Application.Dto;

public record RegisterConfigurationInput(string Name, string Value, string Description, DateTime StartDate, DateTime FinalDate) : 
    BaseConfiguration(Name, Value, Description, StartDate, FinalDate);
