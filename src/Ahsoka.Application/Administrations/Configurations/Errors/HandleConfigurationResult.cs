using Ahsoka.Application.Common.Models;
using Ahsoka.Application.Dto.Administrations.Configurations.ApplicationsErrors;
using Ahsoka.Application.Dto.Common.ApplicationsErrors.Models;
using Ahsoka.Domain.Common.ValuesObjects;

namespace Ahsoka.Application.Administrations.Configurations.Errors;

public static class HandleConfigurationResult
{
    public static void HandleResultConfiguration(Result? result, Notifier notifier)
    {
        var errorsMapping = new Dictionary<DomainErrorCode, ApplicationErrorCode>()
        {
            { DomainErrorCode.Validation, ConfigurationErrorCodes.ConfigurationsValidation}
        };

        foreach (var error in result.Errors)
        {
            errorsMapping.TryGetValue(error.Error, out var value);

            if (value != null)
            {
                notifier.Errors.Add(Dto.Common.ApplicationsErrors.Errors.ConfigurationValidation()
                    .ChangeInnerMessage(error.Message ?? string.Empty));
            }
            else
            {
                notifier.Errors.Add(Dto.Common.ApplicationsErrors.Errors.Generic());
            }
        }

        foreach (var error in result.Warnings)
        {
            errorsMapping.TryGetValue(error.Error, out var value);

            if (value != null)
            {
                notifier.Warnings.Add(Dto.Common.ApplicationsErrors.Errors.ConfigurationValidation());
            }
            else
            {
                notifier.Warnings.Add(Dto.Common.ApplicationsErrors.Errors.Generic());
            }
        }
    }
}
