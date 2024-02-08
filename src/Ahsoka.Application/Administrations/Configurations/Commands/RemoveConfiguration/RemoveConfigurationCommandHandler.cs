using Ahsoka.Application.Common;
using Ahsoka.Application.Common.Attributes;
using Ahsoka.Application.Common.Interfaces;
using Ahsoka.Application.Common.Models;
using Ahsoka.Application.Dto.Administrations.Configurations.ApplicationsErrors;
using Ahsoka.Domain.Entities.Admin.Configurations;
using Ahsoka.Domain.Entities.Admin.Configurations.Repository;
using MediatR;

namespace Ahsoka.Application.Administrations.Configurations.Commands.RemoveConfiguration;

public record RemoveConfigurationCommand(ConfigurationId Id) : IRequest;

public class RemoveConfigurationCommandHandler(IConfigurationRepository repository,
    IUnitOfWork unitOfWork,
    Notifier notifier)
    : BaseCommands(notifier),  IRequestHandler<RemoveConfigurationCommand>
{

    [Transaction]
    [Log]
    public async Task Handle(RemoveConfigurationCommand request, CancellationToken cancellationToken)
    {
        var entity = await repository.GetByIdAsync(request.Id, cancellationToken);

        if (entity is null)
        {
            _notifier.Warnings.Add(ConfigurationErrors.ConfigurationNotFound());
            return;
        }

        entity.Delete();

        await repository.UpdateAsync(entity, cancellationToken);

        await unitOfWork.CommitAsync(cancellationToken);
    }
}


