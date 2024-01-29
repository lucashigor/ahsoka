namespace Ahsoka.Application;

using Ahsoka.Application.Dto;
using FluentValidation;
using MediatR;

public class RequestValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public RequestValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var context = new ValidationContext<TRequest>(request);

        var failures = _validators
            .Select(v => v.Validate(context))
            .SelectMany(result => result.Errors)
            .Where(f => f != null)
            .ToList();

        if (failures.Count == 0) return next();

        var errors = failures
            .Select(validationFailure =>
                new ErrorModel(Errors.Validation().Code, validationFailure.ErrorMessage))
            .ToList();

        throw new ValidationException(errors);
    }
}
