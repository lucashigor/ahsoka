using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Ahsoka.Application.Common.Models.Authorizations;

public class ApplicationUser(HttpContext httpContext)
{
    public Guid UserId { get; } = Guid.Parse(httpContext?.User?.FindFirstValue(ApplicationClaims.Id) ?? "dee240d6-39a1-423b-ac31-10c991759cdd");
    public string Name { get; } = httpContext?.User?.FindFirstValue(ApplicationClaims.Name) ?? "system";
    public bool IsAuthenticated { get; } = httpContext?.User?.Identity?.IsAuthenticated ?? false;
    public IEnumerable<Claim> UserClaims { get; } = httpContext?.User.Claims ?? new HashSet<Claim>();
}
