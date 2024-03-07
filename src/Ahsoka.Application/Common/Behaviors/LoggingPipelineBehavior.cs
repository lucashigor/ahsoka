using Ahsoka.Application.Common.Attributes;
using Ahsoka.Application.Dto.Common.Responses;
using Azure;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
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

        var sw = new Stopwatch();
        sw.Start();

        var response = await next();

        sw.Stop();

        LogInformation(request, "request", sw);

        LogInformation(response, "response", sw);

        return response;
    }

    private void LogInformation(object? request, string type, Stopwatch sw)
    {
        if (request is null) { return; }

        StringBuilder messageRequest;
        object?[] valuesRequest;

        GetMessageToLog(request, out messageRequest, out valuesRequest);

        var parameters = new object[] { type, typeof(TRequest).FullName! }
            .Concat(valuesRequest.Where(x => x is not null).ToArray())
            .Concat(new object[] { sw.ElapsedMilliseconds })
            .ToArray();

        messageRequest.Append(" ProcessTime - {ProcessTime}");

        _logger.LogInformation(messageRequest.ToString(), parameters);
    }

    private void GetMessageToLog(object request, out StringBuilder message, out object?[] values)
    {
        var requestType = request.GetType();
        var properties = requestType.GetProperties();

        message = new();
        message.Append("Type - {type} Command - {command} props:");

        values = new object?[properties.Length];
        for (int i = 0; i < properties.Length; i++)
        {
            var propertyName = properties[i].Name;
            var propertyValue = properties[i].GetValue(request);

            if (!IsSensitive(properties[i]))
            {
                values[i] = propertyValue ?? "";

                message.Append(" " + propertyName + " - {" + propertyName + "}");
            }
        }
    }

    private bool IsSensitive(PropertyInfo property)
    {
        return property.GetCustomAttribute<SensitiveDataAttribute>() != null;
    }
}

