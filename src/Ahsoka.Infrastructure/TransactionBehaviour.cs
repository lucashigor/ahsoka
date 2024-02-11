using Ahsoka.Application.Common.Attributes;
using Ahsoka.Infrastructure.Repositories.Context;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Reflection;

namespace Ahsoka.Infrastructure;

public class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
{
    private readonly ILogger<TransactionBehavior<TRequest, TResponse>> _logger;
    private readonly PrincipalContext _db;
    private readonly IRequestHandler<TRequest, TResponse> _outerHandler;

    public TransactionBehavior(
        ILogger<TransactionBehavior<TRequest, TResponse>> logger,
        PrincipalContext db,
        IRequestHandler<TRequest, TResponse> outerHandler)
    {
        _logger = logger;
        _db = db;
        _outerHandler = outerHandler;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var transactionAttr = _outerHandler
            .GetType()
            ?.GetTypeInfo()
            ?.GetDeclaredMethod(nameof(_outerHandler.Handle))
            ?.GetCustomAttributes(typeof(TransactionAttribute), true);

        if (transactionAttr != null && transactionAttr.Length < 1)
        {
            _logger.LogInformation("Handled {request}", typeof(TRequest).FullName);
            return await next();
        }

        _logger.LogInformation("Opening transaction for {request}", typeof(TRequest));

        var strategy = _db.Database.CreateExecutionStrategy();

        return await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = _db.Database.BeginTransaction(IsolationLevel.ReadCommitted);

            try
            {
                _logger.LogInformation("Executing the {request} request.", typeof(TRequest).FullName);

                var response = await next();

                await transaction.CommitAsync(cancellationToken);

                _logger.LogInformation("Commit {request}", typeof(TRequest).FullName);
                return response;
            }
            catch
            {
                _logger.LogInformation("Rollback {request}", typeof(TRequest).FullName);
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        });
    }
}