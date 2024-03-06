namespace Ahsoka.Domain.Common.ValuesObjects;
using System.Collections.Generic;
using System.Collections.Immutable;

public sealed class DomainResult
{
    public bool IsSuccess => _errors.Count == 0;
    public bool IsFailure => !IsSuccess;

    private readonly ICollection<Notification> _errors;
    public IReadOnlyCollection<Notification> Errors => _errors.ToImmutableArray();

    private readonly ICollection<Notification> _warnings;
    public IReadOnlyCollection<Notification> Warnings => _warnings.ToImmutableArray();

    private readonly ICollection<Notification> _infos;
    public IReadOnlyCollection<Notification> Infos => _infos.ToImmutableArray();

    private DomainResult(ICollection<Notification>? errors,
        ICollection<Notification>? warnings,
        ICollection<Notification>? infos)
    {
        _warnings = warnings ?? [];
        _errors = errors ?? [];
        _infos = infos ?? [];
    }

    public static DomainResult Success(
        ICollection<Notification>? warnings = null, 
        ICollection<Notification>? infos = null) => new(null, warnings, infos);

    public static DomainResult Failure(
        ICollection<Notification>? errors = null, 
        ICollection<Notification>? warnings = null, 
        ICollection<Notification>? infos = null) => new(errors, warnings, infos);
}
