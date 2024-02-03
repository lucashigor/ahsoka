namespace Ahsoka.Application.Common.Models.Authorizations;

public interface ICurrentUserService
{
    ApplicationUser User { get; }
}
