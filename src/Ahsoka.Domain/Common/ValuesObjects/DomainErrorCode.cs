namespace Ahsoka.Domain;

public record DomainErrorCode
{
    protected DomainErrorCode(int value) { Value = value; }

    public int Value { get; init; }

    public override string ToString() => Value.ToString();

    public static implicit operator DomainErrorCode(int value) => new(value);

    public static implicit operator int(DomainErrorCode month) => month.Value;
}
