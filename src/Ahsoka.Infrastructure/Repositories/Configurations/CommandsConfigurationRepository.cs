using Ahsoka.Domain.Entities.Admin.Configurations;
using Ahsoka.Domain.Entities.Admin.Configurations.Repository;
using Ahsoka.Infrastructure.Repositories.Common;
using Ahsoka.Infrastructure.Repositories.Context;

namespace Ahsoka.Infrastructure.Repositories.Configurations;

public class CommandsConfigurationRepository(PrincipalContext context) :
    CommandsBaseRepository<Configuration, ConfigurationId>(context),
    ICommandsConfigurationRepository
{
}

