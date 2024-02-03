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

namespace Ahsoka.Application.Administrations.Configurations.Commands;
public record RegisterConfigurationCommand(string Name, string Value, string Description, DateTime StartDate, DateTime? ExpireDate)
    : BaseConfiguration(Name, Value, Description, StartDate, ExpireDate), IRequest<ConfigurationOutput?>;

public class RegisterConfigurationCommandHandler(IConfigurationRepository repository,
    IUnitOfWork unitOfWork,
    Notifier notifier,
    IConfigurationServices configurationServices,
    ICurrentUserService userService) : BaseCommands(notifier), IRequestHandler<RegisterConfigurationCommand, ConfigurationOutput?>
{
    [Transaction]
    [Log]
    public async Task<ConfigurationOutput?> Handle(RegisterConfigurationCommand request, CancellationToken cancellationToken)
    {
        var item = Configuration.New(request.Name,
            request.Value,
            request.Description,
            request.StartDate,
            request.ExpireDate,
            userService.User.UserId.ToString());

        if (_notifier.Errors.Any())
        {
            return null;
        }

        await configurationServices.Handle(item, cancellationToken);

        await repository.InsertAsync(item, cancellationToken);

        await unitOfWork.CommitAsync(cancellationToken);

        return item.Adapt<ConfigurationOutput>();
    }
}