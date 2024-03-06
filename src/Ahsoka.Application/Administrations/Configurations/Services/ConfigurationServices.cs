using Ahsoka.Application.Dto.Administrations.Configurations.Responses;
using Ahsoka.Application.Dto.Common.Responses;
using Ahsoka.Domain.Entities.Admin.Configurations;
using Ahsoka.Domain.Entities.Admin.Configurations.Repository;

namespace Ahsoka.Application.Administrations.Configurations.Services;
public class ConfigurationServices(IQueriesConfigurationRepository configurationRepository)
    : IConfigurationServices
{
    public async Task<ApplicationResult<ConfigurationOutput>> Handle(Configuration entity, CancellationToken cancellationToken)
    {
        var response = ApplicationResult<ConfigurationOutput>.Success();

        var listWithSameName = await configurationRepository.GetAllByNameAsync(entity.Name, [ConfigurationState.Active, ConfigurationState.Awaiting], cancellationToken);

        if (listWithSameName is not null && listWithSameName.Exists(x => x.Id != entity.Id))
        {
            if (listWithSameName.Exists(x => x.StartDate <= entity.StartDate && x.ExpireDate >= entity.StartDate && x.Id != entity.Id))
            {
                response.AddError(Dto.Common.ApplicationsErrors.Errors.ThereWillCurrentConfigurationStartDate());
            }

            if (listWithSameName.Exists(x => x.StartDate <= entity.ExpireDate && x.ExpireDate >= entity.ExpireDate && x.Id != entity.Id))
            {
                response.AddError(Dto.Common.ApplicationsErrors.Errors.ThereWillCurrentConfigurationEndDate());
            }
        }

        return response;
    }
}
