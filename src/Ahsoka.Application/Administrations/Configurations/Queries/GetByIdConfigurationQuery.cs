using Ahsoka.Application.Dto.Administrations.Configurations.Responses;
using Ahsoka.Domain.Entities.Admin.Configurations;
using Ahsoka.Domain.Entities.Admin.Configurations.Repository;
using Mapster;
using MediatR;

namespace Ahsoka.Application.Administrations.Configurations.Queries;

public record GetConfigurationByIdQuery(ConfigurationId Id) : IRequest<ConfigurationOutput?>;

public class GetByIdConfigurationQueryHandler(IConfigurationRepository repository) 
    : IRequestHandler<GetConfigurationByIdQuery, ConfigurationOutput?>
{
    public async Task<ConfigurationOutput?> Handle(GetConfigurationByIdQuery request, CancellationToken cancellationToken)
    {
        var item = await repository.GetByIdAsync(request.Id, cancellationToken);

        return item.Adapt<ConfigurationOutput?>();
    }
}
