namespace Ahsoka.Domain.Common.ValuesObjects;

public sealed partial record DomainErrorCode
{
    internal int Value { get; set; }
    private DomainErrorCode(int value)
    {
        Value = value;
    }
    public override string ToString() => Value.ToString();

    public static implicit operator int(DomainErrorCode id) => id.Value;
}
