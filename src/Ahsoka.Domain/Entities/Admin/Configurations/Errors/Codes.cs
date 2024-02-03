using Ahsoka.Domain.Common.ValuesObjects;

namespace Ahsoka.Domain.Entities.Admin.Configurations.Errors;

public sealed record ConfigurationsErrorsCodes : DomainErrorCode
{
    private ConfigurationsErrorsCodes(int original) : base(original)
    {
    }

    public static readonly ConfigurationsErrorsCodes Validation = new(2_000);
    public static readonly ConfigurationsErrorsCodes ErrorOnDelete = new(2_001);
    public static readonly ConfigurationsErrorsCodes OnlyDescriptionAllowedToChange = new(2_002);
    public static readonly ConfigurationsErrorsCodes ErrorOnChangeName = new(2_003);


}
