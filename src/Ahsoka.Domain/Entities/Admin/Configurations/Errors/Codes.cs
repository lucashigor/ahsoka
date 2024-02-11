namespace Ahsoka.Domain.Common.ValuesObjects;

public sealed partial record DomainErrorCode
{
    public static readonly DomainErrorCode ErrorOnDelete = new(2_001);
    public static readonly DomainErrorCode OnlyDescriptionAllowedToChange = new(2_002);
    public static readonly DomainErrorCode ErrorOnChangeName = new(2_003);
    public static readonly DomainErrorCode SetExpireDateToToday = new(2_004);
}
