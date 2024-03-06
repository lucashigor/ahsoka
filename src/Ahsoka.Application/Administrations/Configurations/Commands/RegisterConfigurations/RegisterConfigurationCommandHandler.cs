using Ahsoka.Application.Administrations.Configurations.Errors;
using Ahsoka.Application.Administrations.Configurations.Services;
using Ahsoka.Application.Common.Attributes;
using Ahsoka.Application.Common.Interfaces;
using Ahsoka.Application.Common.Models.Authorizations;
using Ahsoka.Application.Dto.Administrations.Configurations.Responses;
using Ahsoka.Application.Dto.Common.Responses;
using Ahsoka.Domain.Entities.Admin.Configurations;
using Ahsoka.Domain.Entities.Admin.Configurations.Repository;
using Mapster;
using MediatR;
using _dto = Ahsoka.Application.Dto.Administrations.Configurations.Requests;

namespace Ahsoka.Application.Administrations.Configurations.Commands.RegisterConfiguration;
public record RegisterConfigurationCommand(_dto.BaseConfiguration BaseConfiguration)
    : BaseConfiguration(BaseConfiguration), IRequest<ApplicationResult<ConfigurationOutput>>;

public class RegisterConfigurationCommandHandler(ICommandsConfigurationRepository repository,
    IUnitOfWork unitOfWork,
    IConfigurationServices configurationServices,
    ICurrentUserService userService) : IRequestHandler<RegisterConfigurationCommand, ApplicationResult<ConfigurationOutput>>
{
    [Transaction]
    [Log]
    public async Task<ApplicationResult<ConfigurationOutput>> Handle(RegisterConfigurationCommand request, CancellationToken cancellationToken)
    {
        var response = ApplicationResult<ConfigurationOutput>.Success();

        var (result, config) = Configuration.New(request.Name,
            request.Value,
            request.Description,
            request.StartDate,
            request.ExpireDate,
            userService.User.UserId.ToString());

        if (result.IsFailure || config is null)
        {
            HandleConfigurationResult.HandleResultConfiguration(result, response);
            return response;
        }

        response = await configurationServices.Handle(config!, cancellationToken);

        if (response.IsFailure)
        {
            return response;
        }

        await repository.InsertAsync(config!, cancellationToken);

        await unitOfWork.CommitAsync(cancellationToken);

        return config!.Adapt<ConfigurationOutput>();
    }
}