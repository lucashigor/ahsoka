
using Ahsoka.Application.Dto;
using Ahsoka.Domain;

namespace Ahsoka.Application;
public class DateValidationServices(IConfigurationRepository configurationRepository,
    Notifier notifier) : IDateValidationServices
{
    private readonly IConfigurationRepository configurationRepository = configurationRepository;
    private readonly Notifier notifier = notifier;

    public async Task Handle(Configuration entity, CancellationToken cancellationToken)
    {
        var listWithSameName = await configurationRepository.GetAllByNameAsync(entity.Name, [ConfigurationStatus.Active, ConfigurationStatus.Awaiting] ,cancellationToken);

        if (listWithSameName is not null && listWithSameName.Exists(x => x.Id != entity.Id))
        {
            if (listWithSameName.Exists(x => x.StartDate < entity.StartDate && x.ExpireDate > entity.StartDate && x.Id != entity.Id))
            {
                notifier.Errors.Add(ConfigurationErrors.ThereWillCurrentConfigurationStartDate());
            }

            if (listWithSameName.Exists(x => x.StartDate < entity.ExpireDate && x.ExpireDate > entity.ExpireDate && x.Id != entity.Id))
            {
                notifier.Errors.Add(ConfigurationErrors.ThereWillCurrentConfigurationEndDate());
            }
        }
    }
}
