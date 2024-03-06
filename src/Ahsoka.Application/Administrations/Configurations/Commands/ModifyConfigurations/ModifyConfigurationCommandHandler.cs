using Ahsoka.Application.Administrations.Configurations.Errors;
using Ahsoka.Application.Administrations.Configurations.Services;
using Ahsoka.Application.Common.Attributes;
using Ahsoka.Application.Common.Extensions;
using Ahsoka.Application.Common.Interfaces;
using Ahsoka.Application.Dto.Administrations.Configurations.Responses;
using Ahsoka.Application.Dto.Common.Responses;
using Ahsoka.Domain.Entities.Admin.Configurations;
using Ahsoka.Domain.Entities.Admin.Configurations.Repository;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;

namespace Ahsoka.Application.Administrations.Configurations.Commands.ModifyConfiguration;

public record ModifyConfigurationCommand(ConfigurationId Id,
    JsonPatchDocument<Dto.Administrations.Configurations.Requests.BaseConfiguration> PatchDocument) : IRequest<ApplicationResult<ConfigurationOutput>>;

public class ModifyConfigurationCommandHandler(ICommandsConfigurationRepository repository,
    IUnitOfWork unitOfWork,
    IConfigurationServices configurationServices) : IRequestHandler<ModifyConfigurationCommand, ApplicationResult<ConfigurationOutput>>
{
    [Transaction]
    [Log]
    public async Task<ApplicationResult<ConfigurationOutput>> Handle(ModifyConfigurationCommand request, CancellationToken cancellationToken)
    {
        var response = ApplicationResult<ConfigurationOutput>.Success();

        response.AddErrors(
            request.PatchDocument.Validate(
               OperationType.Replace,
               [$"/{nameof(Configuration.Name)}",
                   $"/{nameof(Configuration.Value)}",
                   $"/{nameof(Configuration.Description)}",
                   $"/{nameof(Configuration.StartDate)}",
                   $"/{nameof(Configuration.ExpireDate)}"]
               ));

        if (response.IsFailure)
        {
            return response;
        }

        var entity = await repository.GetByIdAsync(request.Id, cancellationToken);

        if (entity == null)
        {
            response.AddError(Dto.Common.ApplicationsErrors.Errors.ConfigurationNotFound());
            return response;
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
            HandleConfigurationResult.HandleResultConfiguration(result, response);
            return response;
        }

        await configurationServices.Handle(entity, cancellationToken);

        if (response.IsFailure)
        {
            return response;
        }

        await repository.UpdateAsync(entity, cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);

        return entity.Adapt<ConfigurationOutput>();
    }
}