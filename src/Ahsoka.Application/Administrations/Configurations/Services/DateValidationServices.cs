
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
        var listWithSameName = await configurationRepository.GetAllByNameAsync(entity.Name, [ConfigurationStatus.Active, ConfigurationStatus.Previous] ,cancellationToken);

        if (listWithSameName is not null && listWithSameName.Where(x => x.Id != entity.Id).Any())
        {
            if (listWithSameName.Where(x => x.StartDate < entity.StartDate && x.FinalDate > entity.StartDate && x.Id != entity.Id).Any())
            {
                notifier.Errors.Add(ConfigurationErrors.ThereWillCurrentConfigurationStartDate());
            }

            if (listWithSameName.Where(x => x.StartDate < entity.FinalDate && x.FinalDate > entity.FinalDate && x.Id != entity.Id).Any())
            {
                notifier.Errors.Add(ConfigurationErrors.ThereWillCurrentConfigurationEndDate());
            }
        }
    }
}
