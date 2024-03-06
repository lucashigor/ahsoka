using Ahsoka.Application.Dto.Administrations.Configurations.Responses;
using Ahsoka.Application.Dto.Common.Responses;
using Ahsoka.Domain.Entities.Admin.Configurations;

namespace Ahsoka.Application.Administrations.Configurations.Services;

public interface IConfigurationServices
{
    Task<ApplicationResult<ConfigurationOutput>> Handle(Configuration entity, CancellationToken cancellationToken);
}
