using System.Linq.Expressions;
using Ahsoka.Domain;
using Microsoft.EntityFrameworkCore;

namespace Ahsoka.Infrastructure;

public class ConfigurationRepository : QueryHelper<Configuration, ConfigurationId>, IConfigurationRepository
{
    public ConfigurationRepository(PrincipalContext context) : base(context)
    {
    }
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

    public Task<SearchOutput<Configuration>> SearchAsync(SearchInput input, CancellationToken cancellationToken)
    {
        Expression<Func<Configuration, bool>> where = x => true;

        if (!string.IsNullOrWhiteSpace(input.Search))
            where = x => x.Name.ToLower().Contains(input.Search.ToLower());

        var items = GetManyPaginated(where,
            input.OrderBy,
            input.Order,
            input.Page,
            input.PerPage,
            out var total)
            .ToList();

        return Task.FromResult(new SearchOutput<Configuration>(input.Page, input.PerPage, total, items!));
    }
    
    public Task<List<Configuration>> GetAllByNameAsync(string name, ConfigurationStatus[] statuses, CancellationToken cancellationToken)
        => Task.FromResult(GetMany(x => x.Name.Equals(name) /*&& statuses.Contains(x.GetStatus())*/).ToList());

    public async Task<Configuration?> GetByNameAsync(string name, ConfigurationStatus[] statuses, CancellationToken cancellationToken)
    => await _dbSet.AsNoTracking().FirstOrDefaultAsync(x => x.Name.Equals(name) &&
        /*statuses.Contains(x.GetStatus()) &&*/
        x.StartDate <= DateTime.UtcNow &&
        (x.ExpireDate == null || (x.ExpireDate != null && x.ExpireDate >= DateTime.UtcNow)));
}

