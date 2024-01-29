namespace Ahsoka.Domain;

public sealed record ConfigurationsErrorsCodes : DomainErrorCode
{
    private ConfigurationsErrorsCodes(int original) : base(original)
    {
    }

    public static readonly ConfigurationsErrorsCodes Validation = new(2_000);
}
