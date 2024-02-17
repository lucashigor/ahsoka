﻿using Ahsoka.Application.Common.Attributes;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Reflection;
using System.Text;

namespace Ahsoka.Application.Common.Behaviors;

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

        StringBuilder message = new();
        message.Append("Command - {RequestType} props:");


        var values = new object?[properties.Length];

        for (int i = 0; i < properties.Length; i++)
        {
            var propertyName = properties[i].Name;
            var propertyValue = properties[i].GetValue(request);

            if (!IsSensitive(properties[i]))
            {
                values[i] = propertyValue;

                message.Append(" " + propertyName + " - {" + propertyName + "}");
            }
        }

        var response = await next();

        var parameters = new object[] { typeof(TRequest).FullName! }.Concat(values.Where(x => x is not null).ToArray()).ToArray();

        _logger.LogInformation(message.ToString(), parameters);

        return response;
    }

    private bool IsSensitive(PropertyInfo property)
    {
        return property.GetCustomAttribute<SensitiveDataAttribute>() != null;
    }
}

