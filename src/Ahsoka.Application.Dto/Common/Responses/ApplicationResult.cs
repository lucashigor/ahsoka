using Ahsoka.Application.Dto.Common.ApplicationsErrors.Models;
using System.Collections.Immutable;

namespace Ahsoka.Application.Dto.Common.Responses;

public class ApplicationResult<T> where T : class
{
    public bool IsSuccess => _errors.Count == 0;
    public bool IsFailure => !IsSuccess;
    public T? Data { get; private set; }

    private readonly ICollection<ErrorModel> _errors;
    public IReadOnlyCollection<ErrorModel> Errors => _errors.ToImmutableArray();

    private readonly ICollection<ErrorModel> _warnings;
    public IReadOnlyCollection<ErrorModel> Warnings => _warnings.ToImmutableArray();

    private readonly ICollection<ErrorModel> _infos;
    public IReadOnlyCollection<ErrorModel> Infos => _infos.ToImmutableArray();

    private ApplicationResult(ICollection<ErrorModel>? errors,
        ICollection<ErrorModel>? warnings,
        ICollection<ErrorModel>? infos,
        T? data)
    {
        _warnings = warnings ?? [];
        _errors = errors ?? [];
        _infos = infos ?? [];
        Data = data;
    }

    public static implicit operator ApplicationResult<T>(T value) => Success(Data: value);

    public static ApplicationResult<T> Success(
        ICollection<ErrorModel>? Warnings = null,
        ICollection<ErrorModel>? Infos = null,
        T? Data = null) => new(null, Warnings, Infos, Data);

    public static ApplicationResult<T> Failure(
        ICollection<ErrorModel>? Errors = null,
        ICollection<ErrorModel>? Warnings = null,
        ICollection<ErrorModel>? Infos = null,
        T? Data = null) => new(Errors, Warnings, Infos, Data);

    public ApplicationResult<T> AddError(ErrorModel Errors)
    {
        _errors.Add(Errors);
        return this;
    }
    public ApplicationResult<T> AddErrors(List<ErrorModel> Errors)
    {
        Errors.ForEach(e => _errors.Add(e));
        return this;
    }

    public ApplicationResult<T> AddWarnings(ErrorModel Errors)
    {
        _warnings.Add(Errors);
        return this;
    }

    public ApplicationResult<T> AddInfos(ErrorModel Errors)
    {
        _infos.Add(Errors);
        return this;
    }

    public ApplicationResult<T> SetData(T data)
    {
        Data = data;
        return this;
    }
}
