namespace Ahsoka.Application;

using Ahsoka.Application.Dto;
using Ahsoka.Domain;
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
        catch (InvalidDomainException ex)
        {
            throw new ValidationException(HandlerInvalidDomainExceptionException(ex));
        }
    }

    protected List<ErrorModel> HandlerInvalidDomainExceptionException(InvalidDomainException ex)
    {
        var ret = new List<ErrorModel>();
        var errors = ex.Message.Split(";");

        foreach (var item in errors)
        {
            ErrorModel message;

            var err = item.Split(":");

            message = GetErrors().GetValueOrDefault(int.Parse(err[0]), Errors.Generic());

            message.ChangeInnerMessage(err[1]);

            ret.Add(message);
        }

        return ret;
    }

    protected Dictionary<int, ErrorModel> GetErrors()
        => new() { 
            { 
                ConfigurationsErrorsCodes.Validation.Value, Errors.Validation()
            }
        };
}
