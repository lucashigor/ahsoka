using Ahsoka.Domain.SeedWork.Repository.CommandRepository;

namespace Ahsoka.Domain.Entities.Admin.Configurations.Repository;

public interface ICommandsConfigurationRepository : ICommandRepository<Configuration, ConfigurationId>
{
}
