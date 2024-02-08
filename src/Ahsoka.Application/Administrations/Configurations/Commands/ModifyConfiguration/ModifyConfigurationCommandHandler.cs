using Ahsoka.Application.Administrations.Configurations.Services;
using Ahsoka.Application.Common;
using Ahsoka.Application.Common.Extensions;
using Ahsoka.Application.Common.Interfaces;
using Ahsoka.Application.Common.Models;
using Ahsoka.Application.Dto.Administrations.Configurations.ApplicationsErrors;
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
    JsonPatchDocument<BaseConfiguration> PatchDocument) : IRequest<ConfigurationOutput?>;

public class ModifyConfigurationCommandHandler(IConfigurationRepository repository,
    IUnitOfWork unitOfWork,
    Notifier notifier,
    IConfigurationServices configurationServices) : BaseCommands(notifier) , IRequestHandler<ModifyConfigurationCommand, ConfigurationOutput?>
{
    public async Task<ConfigurationOutput?> Handle(ModifyConfigurationCommand request, CancellationToken cancellationToken)
    {
        try
        {
            request.PatchDocument.Validate(
                   OperationType.Replace,
                   new List<string> { $"/{nameof(Configuration.Name)}",
                    $"/{nameof(Configuration.Value)}",
                    $"/{nameof(Configuration.Description)}",
                    $"/{nameof(Configuration.StartDate)}",
                    $"/{nameof(Configuration.ExpireDate)}" }
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
            notifier.Errors.Add(ConfigurationErrors.ConfigurationNotFound());
            return null!;
        }

        var oldItem = new BaseConfiguration(
            entity.Name,
            entity.Value,
            entity.Description,
            entity.StartDate,
            entity.ExpireDate);

        request.PatchDocument.ApplyTo(oldItem);

        entity.Update(
            name: oldItem.Name,
            value: oldItem.Value,
            description: oldItem.Description,
            startDate: oldItem.StartDate,
            expireDate: oldItem.ExpireDate);

        await configurationServices.Handle(entity, cancellationToken);

        await repository.UpdateAsync(entity, cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);

        return entity.Adapt<ConfigurationOutput>();
    }
}