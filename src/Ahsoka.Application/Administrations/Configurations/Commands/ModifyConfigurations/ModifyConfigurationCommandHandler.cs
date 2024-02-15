using Ahsoka.Application.Administrations.Configurations.Errors;
using Ahsoka.Application.Administrations.Configurations.Services;
using Ahsoka.Application.Common;
using Ahsoka.Application.Common.Extensions;
using Ahsoka.Application.Common.Interfaces;
using Ahsoka.Application.Common.Models;
using Ahsoka.Application.Dto.Administrations.Configurations.Responses;
using Ahsoka.Application.Dto.Common.ApplicationsErrors.Models;
using Ahsoka.Domain.Entities.Admin.Configurations;
using Ahsoka.Domain.Entities.Admin.Configurations.Repository;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;

namespace Ahsoka.Application.Administrations.Configurations.Commands.ModifyConfiguration;

public record ModifyConfigurationCommand(ConfigurationId Id,
    JsonPatchDocument<Dto.Administrations.Configurations.Requests.BaseConfiguration> PatchDocument) : IRequest<ConfigurationOutput?>;

public class ModifyConfigurationCommandHandler(ICommandsConfigurationRepository repository,
    IUnitOfWork unitOfWork,
    Notifier notifier,
    IConfigurationServices configurationServices) : BaseCommands(notifier), IRequestHandler<ModifyConfigurationCommand, ConfigurationOutput?>
{
    public async Task<ConfigurationOutput?> Handle(ModifyConfigurationCommand request, CancellationToken cancellationToken)
    {
        try
        {
            request.PatchDocument.Validate(
                   OperationType.Replace,
                   [ $"/{nameof(Configuration.Name)}",
                    $"/{nameof(Configuration.Value)}",
                    $"/{nameof(Configuration.Description)}",
                    $"/{nameof(Configuration.StartDate)}",
                    $"/{nameof(Configuration.ExpireDate)}" ]
                   );
        }
        catch (BusinessException ex)
        {
            _notifier.Errors.Add(ex.ErrorCode);
            return null!;
        }

        var entity = await repository.GetByIdAsync(request.Id, cancellationToken);

        if (entity == null)
        {
            _notifier.Errors.Add(Dto.Common.ApplicationsErrors.Errors.ConfigurationNotFound());
            return null!;
        }

        var oldItem = new Dto.Administrations.Configurations.Requests.BaseConfiguration(
            entity.Name,
            entity.Value,
            entity.Description,
            entity.StartDate,
            entity.ExpireDate);

        request.PatchDocument.ApplyTo(oldItem);

        var result = entity.Update(
            name: oldItem.Name,
            value: oldItem.Value,
            description: oldItem.Description,
            startDate: oldItem.StartDate,
            expireDate: oldItem.ExpireDate);

        if (result.IsFailure)
        {
            HandleConfigurationResult.HandleResultConfiguration(result, _notifier);
            return null;
        }

        await configurationServices.Handle(entity, cancellationToken);

        if (_notifier.Errors.Count > 0)
        {
            return null;
        }

        await repository.UpdateAsync(entity, cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);

        return entity.Adapt<ConfigurationOutput>();
    }
}