namespace Ahsoka.Domain;

public sealed record CommonErrorCodes(int original) : DomainErrorCode(original)
{
    public static readonly CommonErrorCodes Validation = new(1_000);
    public static readonly CommonErrorCodes InvalidYear = new(1_001);
    public static readonly CommonErrorCodes InvalidMonth = new(1_002);

    public static implicit operator CommonErrorCodes(int value) => new(value);
    public static implicit operator int(CommonErrorCodes month) => month.Value;
}