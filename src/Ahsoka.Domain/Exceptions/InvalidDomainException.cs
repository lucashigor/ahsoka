namespace Ahsoka.Domain;

public sealed class InvalidDomainException(string? message, DomainErrorCode code) : Exception(message)
{
    public int Code { get; init; } = code.Value;
}
