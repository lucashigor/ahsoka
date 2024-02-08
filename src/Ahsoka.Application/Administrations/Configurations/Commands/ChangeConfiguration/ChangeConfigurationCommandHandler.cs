using Ahsoka.Application.Administrations.Configurations.Services;
using Ahsoka.Application.Common;
using Ahsoka.Application.Common.Attributes;
using Ahsoka.Application.Common.Interfaces;
using Ahsoka.Application.Common.Models;
using Ahsoka.Application.Dto.Administrations.Configurations.ApplicationsErrors;
using Ahsoka.Application.Dto.Administrations.Configurations.Responses;
using Ahsoka.Domain.Entities.Admin.Configurations;
using Ahsoka.Domain.Entities.Admin.Configurations.Repository;
using Mapster;
using MediatR;
using _dto = Ahsoka.Application.Dto.Administrations.Configurations.Requests;

namespace Ahsoka.Application.Administrations.Configurations.Commands.ChangeConfiguration;

public record ChangeConfigurationCommand(ConfigurationId Id, _dto.BaseConfiguration BaseConfiguration)
    : BaseConfiguration(BaseConfiguration), IRequest<ConfigurationOutput?>;

public class ChangeConfigurationCommandHandler(IConfigurationRepository repository,
    IUnitOfWork unitOfWork,
    Notifier notifier,
    IConfigurationServices configurationServices) : BaseCommands(notifier), IRequestHandler<ChangeConfigurationCommand, ConfigurationOutput?>
{
    [Transaction]
    [Log]
    public async Task<ConfigurationOutput?> Handle(ChangeConfigurationCommand request, CancellationToken cancellationToken)
    {
        var entity = await repository.GetByIdAsync(request.Id, cancellationToken);

        if (entity is null)
        {
            _notifier.Warnings.Add(ConfigurationErrors.ConfigurationNotFound());
            return null;
        }

        entity.Update(request.Name,
            request.Value,
            request.Description,
            request.StartDate,
            request.ExpireDate);

        await configurationServices.Handle(entity, cancellationToken);

        await repository.UpdateAsync(entity, cancellationToken);

        await unitOfWork.CommitAsync(cancellationToken);

        return entity.Adapt<ConfigurationOutput>();
    }
}