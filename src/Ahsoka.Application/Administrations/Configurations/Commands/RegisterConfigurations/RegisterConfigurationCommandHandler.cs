using Ahsoka.Application.Administrations.Configurations.Services;
using Ahsoka.Application.Common;
using Ahsoka.Application.Common.Attributes;
using Ahsoka.Application.Common.Interfaces;
using Ahsoka.Application.Common.Models;
using Ahsoka.Application.Common.Models.Authorizations;
using Ahsoka.Application.Dto.Administrations.Configurations.Responses;
using Ahsoka.Domain.Entities.Admin.Configurations;
using Ahsoka.Domain.Entities.Admin.Configurations.Repository;
using Mapster;
using MediatR;
using _dto = Ahsoka.Application.Dto.Administrations.Configurations.Requests;
using Ahsoka.Application.Administrations.Configurations.Errors;

namespace Ahsoka.Application.Administrations.Configurations.Commands.RegisterConfiguration;
public record RegisterConfigurationCommand(_dto.BaseConfiguration BaseConfiguration)
    : BaseConfiguration(BaseConfiguration), IRequest<ConfigurationOutput?>;

public class RegisterConfigurationCommandHandler(IConfigurationRepository repository,
    IUnitOfWork unitOfWork,
    Notifier notifier,
    IConfigurationServices configurationServices,
    ICurrentUserService userService) : BaseCommands(notifier), IRequestHandler<RegisterConfigurationCommand, ConfigurationOutput?>
{
    private readonly IConfigurationRepository _configurationRepository = repository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IConfigurationServices _configurationServices = configurationServices;
    private readonly ICurrentUserService _userService = userService;

    [Transaction]
    [Log]
    public async Task<ConfigurationOutput?> Handle(RegisterConfigurationCommand request, CancellationToken cancellationToken)
    {
        var (result, config) = Configuration.New(request.Name,
            request.Value,
            request.Description,
            request.StartDate,
            request.ExpireDate,
            _userService.User.UserId.ToString());

        if (result.IsFailure || config is null)
        {
            HandleConfigurationResult.HandleResultConfiguration(result, notifier);
            return null;
        }

        await _configurationServices.Handle(config!, cancellationToken);

        if (_notifier.Errors.Count > 0)
        {
            return null;
        }

        await _configurationRepository.InsertAsync(config!, cancellationToken);

        await _unitOfWork.CommitAsync(cancellationToken);

        return config!.Adapt<ConfigurationOutput>();
    }
}