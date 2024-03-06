using Ahsoka.Application.Dto.Administrations.Configurations.ApplicationsErrors;
using Ahsoka.Application.Dto.Common.ApplicationsErrors.Models;
using Ahsoka.Application.Dto.Common.Responses;
using Ahsoka.Domain.Common.ValuesObjects;

namespace Ahsoka.Application.Administrations.Configurations.Errors;

public static class HandleConfigurationResult
{
    public static void HandleResultConfiguration<T>(DomainResult result, ApplicationResult<T> notifier) where T : class
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
                notifier.AddError(Dto.Common.ApplicationsErrors.Errors.ConfigurationValidation()
                    .ChangeInnerMessage(error.Message ?? string.Empty));
            }
            else
            {
                notifier.AddError(Dto.Common.ApplicationsErrors.Errors.Generic());
            }
        }

        foreach (var error in result.Warnings)
        {
            errorsMapping.TryGetValue(error.Error, out var value);

            if (value != null)
            {
                notifier.AddError(Dto.Common.ApplicationsErrors.Errors.ConfigurationValidation());
            }
            else
            {
                notifier.AddError(Dto.Common.ApplicationsErrors.Errors.Generic());
            }
        }
    }
}
