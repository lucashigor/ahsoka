using Ahsoka.Domain.Entities.Admin.Configurations;
using Ahsoka.Domain.Entities.Admin.Configurations.Repository;
using Ahsoka.Domain.SeedWork.Repository.ISearchableRepository;
using Ahsoka.Infrastructure.Repositories.Common;
using Ahsoka.Infrastructure.Repositories.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Ahsoka.Infrastructure.Repositories.Configurations;

public class QueriesConfigurationRepository(PrincipalContext context) : QueryHelper<Configuration, ConfigurationId>(context), IQueriesConfigurationRepository
{
    public async Task<Configuration?> GetByIdAsync(ConfigurationId id, CancellationToken cancellationToken)
    => await _dbSet
        .AsNoTracking()
        .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task<SearchOutput<Configuration>> SearchAsync(SearchInput input, CancellationToken cancellationToken)
    {
        Expression<Func<Configuration, bool>> where = x => x.IsDeleted == false;

        if (!string.IsNullOrWhiteSpace(input.Search))
            where = x => x.IsDeleted == false && x.Name.Contains(input.Search, StringComparison.CurrentCultureIgnoreCase);

        var items = GetManyPaginated(where,
            input.OrderBy,
            input.Order,
            input.Page,
            input.PerPage,
            out var total)
            .ToList();

        return Task.FromResult(new SearchOutput<Configuration>(input.Page, input.PerPage, total, items!));
    }

    public Task<List<Configuration>> GetAllByNameAsync(string name, ConfigurationState[] statuses, CancellationToken cancellationToken)
        => Task.FromResult(GetMany(x => x.Name.Equals(name) && x.IsDeleted == false/*&& statuses.Contains(x.GetStatus())*/).ToList());

    public async Task<Configuration?> GetByNameAsync(string name, ConfigurationState[] statuses, CancellationToken cancellationToken)
    => await _dbSet.AsNoTracking().FirstOrDefaultAsync(x => x.Name.Equals(name) &&
        /*statuses.Contains(x.GetStatus()) &&*/
        x.StartDate <= DateTime.UtcNow &&
        (x.ExpireDate == null || x.ExpireDate != null && x.ExpireDate >= DateTime.UtcNow), cancellationToken);
}

