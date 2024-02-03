using Ahsoka.Domain.Common.ValuesObjects;

namespace Ahsoka.Domain.Exceptions;

public sealed class InvalidDomainException(string? message, DomainErrorCode code) : Exception(message)
{
    public int Code { get; init; } = code.Value;
}
