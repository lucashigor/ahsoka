namespace Ahsoka.Application.Common.Models.Authorizations;
using System.Security.Claims;

public record ApplicationClaims
{
    public static readonly string Id = ClaimTypes.NameIdentifier;
    public static readonly string Name = ClaimTypes.Name;
    public static readonly string Role = ClaimTypes.Role;
}
