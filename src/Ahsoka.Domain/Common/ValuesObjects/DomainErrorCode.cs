namespace Ahsoka.Domain.Common.ValuesObjects;

public sealed partial record DomainErrorCode(int Value)
{
    public override string ToString() => Value.ToString();
}
