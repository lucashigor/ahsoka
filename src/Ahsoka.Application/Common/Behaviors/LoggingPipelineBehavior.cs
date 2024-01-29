using System.Reflection;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ahsoka.Application;

public class LoggingPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
{
    private readonly ILogger<LoggingPipelineBehavior<TRequest, TResponse>> _logger;
    private readonly IRequestHandler<TRequest, TResponse> _outerHandler;

    public LoggingPipelineBehavior(
        ILogger<LoggingPipelineBehavior<TRequest, TResponse>> logger,
        IRequestHandler<TRequest, TResponse> outerHandler)
    {
        _logger = logger;
        _outerHandler = outerHandler;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var transactionAttr = _outerHandler
            .GetType()
            ?.GetTypeInfo()
            ?.GetDeclaredMethod(nameof(_outerHandler.Handle))
            ?.GetCustomAttributes(typeof(LogAttribute), true);

        if (transactionAttr != null && transactionAttr.Length < 1)
        {
            return await next();
        }

        var requestType = request.GetType();
        var properties = requestType.GetProperties();

        var message = "Command - {RequestType} props:";

        var values = new object?[properties.Length];

        for (int i = 0; i < properties.Length; i++)
        {
            var propertyName = properties[i].Name;
            var propertyValue = properties[i].GetValue(request);

            if (IsSensitive(properties[i]) is false)
            {
                values[i] = propertyValue;

                message += " " + propertyName + " - {" + propertyName + "}";
            }
        }

        var response = await next();

        var parameters = new object[] { typeof(TRequest).FullName }.Concat(values.Where(x => x is not null).ToArray()).ToArray();

        _logger.LogInformation(message, parameters);

        return response;
    }

    private bool IsSensitive(PropertyInfo property)
    {
        return property.GetCustomAttribute<SensitiveDataAttribute>() != null;
    }
}

