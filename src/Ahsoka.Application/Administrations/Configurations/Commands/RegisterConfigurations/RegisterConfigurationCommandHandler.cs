﻿using Ahsoka.Application.Administrations.Configurations.Errors;
using Ahsoka.Application.Administrations.Configurations.Services;
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

namespace Ahsoka.Application.Administrations.Configurations.Commands.RegisterConfiguration;
public record RegisterConfigurationCommand(_dto.BaseConfiguration BaseConfiguration)
    : BaseConfiguration(BaseConfiguration), IRequest<ConfigurationOutput?>;

public class RegisterConfigurationCommandHandler(ICommandsConfigurationRepository repository,
    IUnitOfWork unitOfWork,
    Notifier notifier,
    IConfigurationServices configurationServices,
    ICurrentUserService userService) : IRequestHandler<RegisterConfigurationCommand, ConfigurationOutput?>
{
    [Transaction]
    [Log]
    public async Task<ConfigurationOutput?> Handle(RegisterConfigurationCommand request, CancellationToken cancellationToken)
    {
        var (result, config) = Configuration.New(request.Name,
            request.Value,
            request.Description,
            request.StartDate,
            request.ExpireDate,
            userService.User.UserId.ToString());

        if (result.IsFailure || config is null)
        {
            HandleConfigurationResult.HandleResultConfiguration(result, notifier);
            return null;
        }

        await configurationServices.Handle(config!, cancellationToken);

        if (notifier.Errors.Count > 0)
        {
            return null;
        }

        await repository.InsertAsync(config!, cancellationToken);

        await unitOfWork.CommitAsync(cancellationToken);

        return config!.Adapt<ConfigurationOutput>();
    }
}