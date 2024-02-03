using Ahsoka.Application.Common;
using Ahsoka.Application.Common.Models;
using Ahsoka.Application.Dto.Administrations.Configurations.ApplicationsErrors;
using Ahsoka.Application.Dto.Administrations.Configurations.Responses;
using Ahsoka.Domain.Entities.Admin.Configurations;
using Ahsoka.Domain.Entities.Admin.Configurations.Repository;
using Mapster;
using MediatR;

namespace Ahsoka.Application.Administrations.Configurations.Queries;

public record GetConfigurationByIdQuery(ConfigurationId Id) : IRequest<ConfigurationOutput>;

public class GetByIdConfigurationQueryHandler : BaseCommands, IRequestHandler<GetConfigurationByIdQuery, ConfigurationOutput>
{
    public IConfigurationRepository _repository;
    public GetByIdConfigurationQueryHandler(IConfigurationRepository repository,
        Notifier notifier) : base(notifier)
    {
        _repository = repository;
    }

    public async Task<ConfigurationOutput> Handle(GetConfigurationByIdQuery request, CancellationToken cancellationToken)
    {
        var item = await _repository.GetByIdAsync(request.Id, cancellationToken);

        if (item is null)
        {
            _notifier.Warnings.Add(ConfigurationErrors.ConfigurationNotFound());

            return null!;
        }

        return item.Adapt<ConfigurationOutput>();
    }
}
