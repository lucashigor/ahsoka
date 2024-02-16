using Ahsoka.Application.Administrations.Configurations.Errors;
using Ahsoka.Application.Common.Attributes;
using Ahsoka.Application.Common.Interfaces;
using Ahsoka.Application.Common.Models;
using Ahsoka.Domain.Entities.Admin.Configurations;
using Ahsoka.Domain.Entities.Admin.Configurations.Repository;
using MediatR;

namespace Ahsoka.Application.Administrations.Configurations.Commands.RemoveConfiguration;

public record RemoveConfigurationCommand(ConfigurationId Id) : IRequest;

public class RemoveConfigurationCommandHandler(ICommandsConfigurationRepository repository,
    IUnitOfWork unitOfWork,
    Notifier notifier) : IRequestHandler<RemoveConfigurationCommand>
{
    private readonly ICommandsConfigurationRepository _repository = repository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly Notifier _notifier = notifier;


    [Transaction]
    [Log]
    public async Task Handle(RemoveConfigurationCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken);

        if (entity is null)
        {
            _notifier.Warnings.Add(Dto.Common.ApplicationsErrors.Errors.ConfigurationNotFound());
            return;
        }

        var result = entity.Delete();

        HandleConfigurationResult.HandleResultConfiguration(result, _notifier);

        if (_notifier.Errors.Count > 0)
        {
            return;
        }

        await _repository.UpdateAsync(entity, cancellationToken);

        await _unitOfWork.CommitAsync(cancellationToken);
    }
}


