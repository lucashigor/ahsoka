using Ahsoka.Domain;

namespace Ahsoka.Application;

public interface IDateValidationServices
{
    Task Handle(Configuration entity, CancellationToken cancellationToken);
}
