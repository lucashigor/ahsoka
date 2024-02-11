using Ahsoka.Application.Administrations.Configurations.Errors;
using Ahsoka.Application.Common;
using Ahsoka.Application.Common.Attributes;
using Ahsoka.Application.Common.Interfaces;
using Ahsoka.Application.Common.Models;
using Ahsoka.Domain.Entities.Admin.Configurations;
using Ahsoka.Domain.Entities.Admin.Configurations.Repository;
using MediatR;

namespace Ahsoka.Application.Administrations.Configurations.Commands.RemoveConfiguration;

public record RemoveConfigurationCommand(ConfigurationId Id) : IRequest;

public class RemoveConfigurationCommandHandler(IConfigurationRepository repository,
    IUnitOfWork unitOfWork,
    Notifier notifier)
    : BaseCommands(notifier), IRequestHandler<RemoveConfigurationCommand>
{

    [Transaction]
    [Log]
    public async Task Handle(RemoveConfigurationCommand request, CancellationToken cancellationToken)
    {
        var entity = await repository.GetByIdAsync(request.Id, cancellationToken);

        if (entity is null)
        {
            _notifier.Warnings.Add(Dto.Common.ApplicationsErrors.Errors.ConfigurationNotFound());
            return;
        }

        var result = entity.Delete();

        HandleConfigurationResult.HandleResultConfiguration(result, notifier);

        if (_notifier.Errors.Count > 0)
        {
            return;
        }

        await repository.UpdateAsync(entity, cancellationToken);

        await unitOfWork.CommitAsync(cancellationToken);
    }
}


