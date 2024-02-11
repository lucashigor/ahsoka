namespace Ahsoka.Domain.Common.ValuesObjects;
using System.Collections.Generic;
using System.Collections.Immutable;

public class Result
{
    public bool IsSuccess { get; private set; }
    public bool IsFailure => !IsSuccess;

    private readonly ICollection<Notification> _errors;
    public IReadOnlyCollection<Notification> Errors => _errors.ToImmutableArray();

    private readonly ICollection<Notification> _warnings;
    public IReadOnlyCollection<Notification> Warnings => _warnings.ToImmutableArray();

    public Result(bool isSuccess,
        ICollection<Notification>? warnings,
        ICollection<Notification>? errors)
    {
        IsSuccess = isSuccess;
        _warnings = warnings ?? new HashSet<Notification>();
        _errors = errors ?? new HashSet<Notification>();
    }

    public static Result Success() => new(true, null, null);
    public static Result Success(ICollection<Notification> warnings) => new(true, warnings, null);
    public static Result Failure(ICollection<Notification> errors, ICollection<Notification> warnings) => new(false, warnings, errors);
    public static Result Failure(ICollection<Notification> errors) => new(false, null, errors);
}
