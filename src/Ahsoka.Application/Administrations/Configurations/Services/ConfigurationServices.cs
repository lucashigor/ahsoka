using Ahsoka.Application.Common.Models;
using Ahsoka.Application.Dto.Administrations.Configurations.ApplicationsErrors;
using Ahsoka.Domain.Entities.Admin.Configurations;
using Ahsoka.Domain.Entities.Admin.Configurations.Repository;

namespace Ahsoka.Application.Administrations.Configurations.Services;
public class ConfigurationServices(IConfigurationRepository configurationRepository,
    Notifier notifier) : IConfigurationServices
{
    public async Task Handle(Configuration entity, CancellationToken cancellationToken)
    {
        var listWithSameName = await configurationRepository.GetAllByNameAsync(entity.Name, [ConfigurationStatus.Active, ConfigurationStatus.Awaiting], cancellationToken);

        if (listWithSameName is not null && listWithSameName.Exists(x => x.Id != entity.Id))
        {
            if (listWithSameName.Exists(x => x.StartDate < entity.StartDate && x.ExpireDate > entity.StartDate && x.Id != entity.Id))
            {
                notifier.Errors.Add(Ahsoka.Application.Dto.Common.ApplicationsErrors.Errors.ThereWillCurrentConfigurationStartDate());
            }

            if (listWithSameName.Exists(x => x.StartDate < entity.ExpireDate && x.ExpireDate > entity.ExpireDate && x.Id != entity.Id))
            {
                notifier.Errors.Add(Ahsoka.Application.Dto.Common.ApplicationsErrors.Errors.ThereWillCurrentConfigurationEndDate());
            }
        }
    }
}
