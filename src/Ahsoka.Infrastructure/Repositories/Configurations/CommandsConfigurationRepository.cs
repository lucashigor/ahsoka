using Ahsoka.Domain.Entities.Admin.Configurations;
using Ahsoka.Domain.Entities.Admin.Configurations.Repository;
using Ahsoka.Infrastructure.Repositories.Common;
using Ahsoka.Infrastructure.Repositories.Context;
using Microsoft.EntityFrameworkCore;

namespace Ahsoka.Infrastructure.Repositories.Configurations;

public class CommandsConfigurationRepository(PrincipalContext context) :
    QueryHelper<Configuration, ConfigurationId>(context),
    ICommandsConfigurationRepository
{
    public async Task InsertAsync(Configuration entity, CancellationToken cancellationToken)
        => await _dbSet.AddAsync(entity, cancellationToken);

    public Task UpdateAsync(Configuration entity, CancellationToken cancellationToken)
    {
        _dbSet.Attach(entity);
        _dbSet.Update(entity);

        return Task.CompletedTask;
    }

    public Task DeleteAsync(Configuration entity, CancellationToken cancellationToken)
        => Task.FromResult(_dbSet.Remove(entity));

    public async Task DeleteAsync(ConfigurationId id, CancellationToken cancellationToken)
    {
        var ids = new object[] { id };
        var item = await _dbSet.FindAsync(ids, cancellationToken);

        if (item != null)
        {
            _dbSet.Remove(item);
        }
    }

    public async Task<Configuration?> GetByIdAsync(ConfigurationId id, CancellationToken cancellationToken)
    => await _dbSet
        .AsNoTracking()
        .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
}

