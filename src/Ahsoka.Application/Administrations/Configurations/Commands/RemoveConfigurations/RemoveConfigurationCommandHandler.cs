using Ahsoka.Application.Administrations.Configurations.Errors;
using Ahsoka.Application.Common.Attributes;
using Ahsoka.Application.Common.Interfaces;
using Ahsoka.Application.Dto.Common.Responses;
using Ahsoka.Domain.Entities.Admin.Configurations;
using Ahsoka.Domain.Entities.Admin.Configurations.Repository;
using MediatR;

namespace Ahsoka.Application.Administrations.Configurations.Commands.RemoveConfiguration;

public record RemoveConfigurationCommand(ConfigurationId Id) : IRequest<ApplicationResult<object>>;

public class RemoveConfigurationCommandHandler(ICommandsConfigurationRepository repository,
    IUnitOfWork unitOfWork) : IRequestHandler<RemoveConfigurationCommand, ApplicationResult<object>>
{
    private readonly ICommandsConfigurationRepository _repository = repository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    [Transaction]
    [Log]
    public async Task<ApplicationResult<object>> Handle(RemoveConfigurationCommand request, CancellationToken cancellationToken)
    {
        var response = ApplicationResult<object>.Success();

        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken);

        if (entity is null)
        {
            response.AddWarnings(Dto.Common.ApplicationsErrors.Errors.ConfigurationNotFound());
            return response;
        }

        var result = entity.Delete();

        HandleConfigurationResult.HandleResultConfiguration(result, response);

        if (response.IsFailure)
        {
            return response;
        }

        await _repository.UpdateAsync(entity, cancellationToken);

        await _unitOfWork.CommitAsync(cancellationToken);
        return response;
    }
}


