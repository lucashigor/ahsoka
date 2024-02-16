namespace Ahsoka.Application.Common.Behaviors;

using Ahsoka.Application.Dto.Common.ApplicationsErrors;
using Ahsoka.Application.Dto.Common.ApplicationsErrors.Models;
using Ahsoka.Domain.Common.ValuesObjects;
using MediatR;

public class RequestExceptionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        try
        {
            var response = await next();
            return response;
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    protected Dictionary<int, ErrorModel> GetErrors()
        => new() {
            {
                DomainErrorCode.Validation, Errors.Validation()
            }
        };
}
