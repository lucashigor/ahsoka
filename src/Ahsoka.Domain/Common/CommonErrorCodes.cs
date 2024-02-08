namespace Ahsoka.Domain.Common.ValuesObjects;

public sealed partial record DomainErrorCode
{
    public static readonly DomainErrorCode Validation = new(1_000);
    public static readonly DomainErrorCode InvalidYear = new(1_001);
    public static readonly DomainErrorCode InvalidMonth = new(1_002);
}