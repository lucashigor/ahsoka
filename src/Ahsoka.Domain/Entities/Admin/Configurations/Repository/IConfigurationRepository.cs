namespace Ahsoka.Domain;

public interface IConfigurationRepository : IRepository<Configuration>, ISearchableRepository<Configuration, SearchInput>
{
    Task<List<Configuration>> GetAllByNameAsync(string name,
        ConfigurationStatus[] statuses,
        CancellationToken cancellationToken);
    Task<Configuration?> GetByNameAsync(string name,
        ConfigurationStatus[] statuses,
        CancellationToken cancellationToken);
}
