using Ahsoka.Application.Administrations.Configurations.Errors;
using Ahsoka.Application.Administrations.Configurations.Services;
using Ahsoka.Application.Common.Attributes;
using Ahsoka.Application.Common.Interfaces;
using Ahsoka.Application.Common.Models;
using Ahsoka.Application.Dto.Administrations.Configurations.Responses;
using Ahsoka.Application.Dto.Common.Responses;
using Ahsoka.Domain.Entities.Admin.Configurations;
using Ahsoka.Domain.Entities.Admin.Configurations.Repository;
using Mapster;
using MediatR;
using _dto = Ahsoka.Application.Dto.Administrations.Configurations.Requests;

namespace Ahsoka.Application.Administrations.Configurations.Commands.ChangeConfiguration;

public record ChangeConfigurationCommand(ConfigurationId Id, _dto.BaseConfiguration BaseConfiguration)
    : BaseConfiguration(BaseConfiguration), IRequest<ApplicationResult<ConfigurationOutput>>;

public class ChangeConfigurationCommandHandler(ICommandsConfigurationRepository repository,
    IUnitOfWork unitOfWork,
    IConfigurationServices configurationServices) : IRequestHandler<ChangeConfigurationCommand, ApplicationResult<ConfigurationOutput>>
{
    [Transaction]
    [Log]
    public async Task<ApplicationResult<ConfigurationOutput>> Handle(ChangeConfigurationCommand request, CancellationToken cancellationToken)
    {
        var response = ApplicationResult<ConfigurationOutput>.Success();

        var entity = await repository.GetByIdAsync(request.Id, cancellationToken);

        if (entity is null)
        {
            response.AddError(Dto.Common.ApplicationsErrors.Errors.ConfigurationNotFound());
            return response;
        }

        var result = entity.Update(request.Name,
            request.Value,
            request.Description,
            request.StartDate,
            request.ExpireDate);

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