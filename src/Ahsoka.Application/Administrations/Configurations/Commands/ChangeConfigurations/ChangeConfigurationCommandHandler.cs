using Ahsoka.Application.Administrations.Configurations.Errors;
using Ahsoka.Application.Administrations.Configurations.Services;
using Ahsoka.Application.Common.Attributes;
using Ahsoka.Application.Common.Interfaces;
using Ahsoka.Application.Common.Models;
using Ahsoka.Application.Dto.Administrations.Configurations.Responses;
using Ahsoka.Domain.Entities.Admin.Configurations;
using Ahsoka.Domain.Entities.Admin.Configurations.Repository;
using Mapster;
using MediatR;
using _dto = Ahsoka.Application.Dto.Administrations.Configurations.Requests;

namespace Ahsoka.Application.Administrations.Configurations.Commands.ChangeConfiguration;

public record ChangeConfigurationCommand(ConfigurationId Id, _dto.BaseConfiguration BaseConfiguration)
    : BaseConfiguration(BaseConfiguration), IRequest<ConfigurationOutput?>;

public class ChangeConfigurationCommandHandler(ICommandsConfigurationRepository repository,
    IUnitOfWork unitOfWork,
    Notifier _notifier,
    IConfigurationServices configurationServices) : IRequestHandler<ChangeConfigurationCommand, ConfigurationOutput?>
{
    [Transaction]
    [Log]
    public async Task<ConfigurationOutput?> Handle(ChangeConfigurationCommand request, CancellationToken cancellationToken)
    {
        var entity = await repository.GetByIdAsync(request.Id, cancellationToken);

        if (entity is null)
        {
            _notifier.Errors.Add(Dto.Common.ApplicationsErrors.Errors.ConfigurationNotFound());
            return null;
        }

        var result = entity.Update(request.Name,
            request.Value,
            request.Description,
            request.StartDate,
            request.ExpireDate);

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