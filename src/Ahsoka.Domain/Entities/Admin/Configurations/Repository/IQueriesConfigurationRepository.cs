using Ahsoka.Domain.SeedWork.Repository.ISearchableRepository;

namespace Ahsoka.Domain.Entities.Admin.Configurations.Repository;

public interface IQueriesConfigurationRepository : 
    IResearchableRepository<Configuration, ConfigurationId, SearchInput>
{
    Task<List<Configuration>> GetAllByNameAsync(string name,
        ConfigurationState[] statuses,
        CancellationToken cancellationToken);
    Task<Configuration?> GetByNameAsync(string name,
        ConfigurationState[] statuses,
        CancellationToken cancellationToken);
}
