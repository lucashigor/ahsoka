using Ahsoka.Application.Dto;
using Ahsoka.Domain;
using Mapster;
using MediatR;

namespace Ahsoka.Application;
public record RegisterConfigurationCommand : BaseConfiguration, IRequest<ConfigurationOutput?>
{
    public RegisterConfigurationCommand(string Name, string Value, string Description, DateTimeOffset StartDate, DateTimeOffset? FinalDate) : base(Name, Value, Description, StartDate, FinalDate)
    {
    }
}

public class RegisterConfigurationCommandHandler(IConfigurationRepository repository,
    IUnitOfWork unitOfWork,
    Notifier notifier,
    IDateValidationServices configurationServices,
    ICurrentUserService userService) : BaseCommands(notifier), IRequestHandler<RegisterConfigurationCommand, ConfigurationOutput?>
{
    private readonly IConfigurationRepository repository = repository;
    private readonly IUnitOfWork unitOfWork = unitOfWork;
    private readonly IDateValidationServices _configurationServices = configurationServices;
    private readonly ICurrentUserService _userService = userService;

    [Transaction]
    [Log]
    public async Task<ConfigurationOutput?> Handle(RegisterConfigurationCommand request, CancellationToken cancellationToken)
    {
        var item = Configuration.New(request.Name,
            request.Value,
            request.Description,
            request.StartDate,
            request.FinalDate,
            _userService.User.UserId.ToString());

        if (_notifier.Errors.Any())
        {
            return null;
        }

        await _configurationServices.Handle(item, cancellationToken);

        await repository.InsertAsync(item, cancellationToken);

        await unitOfWork.CommitAsync(cancellationToken);

        return item.Adapt<ConfigurationOutput>();
    }
}