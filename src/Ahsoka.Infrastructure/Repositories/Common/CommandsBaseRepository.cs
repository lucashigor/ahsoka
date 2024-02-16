using Ahsoka.Domain.SeedWork;
using Ahsoka.Infrastructure.Repositories.Context;
using Microsoft.EntityFrameworkCore;

namespace Ahsoka.Infrastructure.Repositories.Common;

public abstract class CommandsBaseRepository<TEntity, TEntityId>(PrincipalContext context)
    where TEntity : Entity<TEntityId>
    where TEntityId : IEquatable<TEntityId>
{
    protected readonly DbSet<TEntity> _dbSet = context.Set<TEntity>();

    public async virtual Task InsertAsync(TEntity entity, CancellationToken cancellationToken)
        => await _dbSet.AddAsync(entity, cancellationToken);

    public Task UpdateAsync(TEntity entity, CancellationToken cancellationToken)
    {
        _dbSet.Attach(entity);
        _dbSet.Update(entity);

        return Task.CompletedTask;
    }

    public virtual Task DeleteAsync(TEntity entity, CancellationToken cancellationToken)
        => Task.FromResult(_dbSet.Remove(entity));

    public virtual async Task DeleteAsync(TEntityId id, CancellationToken cancellationToken)
    {
        var ids = new object[] { id };
        var item = await _dbSet.FindAsync(ids, cancellationToken);

        if (item != null)
        {
            _dbSet.Remove(item);
        }
    }

    public virtual async Task<TEntity?> GetByIdAsync(TEntityId id, CancellationToken cancellationToken)
    => await _dbSet
        .AsNoTracking()
        .FirstOrDefaultAsync(x => x.Id.Equals(id), cancellationToken);
}
