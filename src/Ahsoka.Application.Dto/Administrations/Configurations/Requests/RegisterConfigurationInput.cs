
namespace Ahsoka.Application.Dto;

public record RegisterConfigurationInput : BaseConfiguration
{
    public RegisterConfigurationInput(string Name, string Value, string Description, DateTimeOffset StartDate, DateTimeOffset FinalDate) : base(Name, Value, Description, StartDate, FinalDate)
    {
    }
}
