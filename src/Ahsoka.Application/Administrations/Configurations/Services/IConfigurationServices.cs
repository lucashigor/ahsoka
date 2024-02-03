using Ahsoka.Domain.Entities.Admin.Configurations;

namespace Ahsoka.Application.Administrations.Configurations.Services;

public interface IConfigurationServices
{
    Task Handle(Configuration entity, CancellationToken cancellationToken);
}
