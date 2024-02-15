using Ahsoka.Application.Common.Interfaces;
using Ahsoka.Domain.SeedWork;
using Ahsoka.Infrastructure.Repositories.Context;
using Microsoft.EntityFrameworkCore;

namespace Ahsoka.Infrastructure.Repositories.Common;

public class UnitOfWork(PrincipalContext _context) : IUnitOfWork
{
    public async Task CommitAsync(CancellationToken cancellationToken)
    {
        await _context.SaveChangesAsync(cancellationToken);

        await DispatchDomainEventsAsync();
    }
    public Task RollbackAsync(CancellationToken cancellationToken)
    {
        _context.ChangeTracker.Entries()
        .Where(e => e.Entity != null).ToList()
        .ForEach(e => e.State = EntityState.Detached);

        return Task.CompletedTask;
    }

    private Task DispatchDomainEventsAsync()
    {
        var domainEntities = _context.ChangeTracker
            .Entries<IAggregateRoot>()
            .Where(x => x.Entity.Events != null && x.Entity.Events.Any());

        var domainEvents = domainEntities
            .SelectMany(x => x.Entity.Events)
            .ToList();

        domainEntities.ToList()
            .ForEach(entity => entity.Entity.ClearEvents());

        foreach (var domainEvent in domainEvents.OrderBy(x => x.EventDateUtc))
        { }

        return Task.CompletedTask;

        //await _mediator.Publish(domainEvent);
    }
}
