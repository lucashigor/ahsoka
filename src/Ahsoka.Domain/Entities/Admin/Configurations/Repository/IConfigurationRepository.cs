using Ahsoka.Domain.SeedWork.Repository;
using Ahsoka.Domain.SeedWork.Repository.ISearchableRepository;

namespace Ahsoka.Domain.Entities.Admin.Configurations.Repository;

public interface IConfigurationRepository : IRepository<Configuration, ConfigurationId>,
    ISearchableRepository<Configuration, ConfigurationId, SearchInput>
{
    Task<List<Configuration>> GetAllByNameAsync(string name,
        ConfigurationStatus[] statuses,
        CancellationToken cancellationToken);
    Task<Configuration?> GetByNameAsync(string name,
        ConfigurationStatus[] statuses,
        CancellationToken cancellationToken);
}
