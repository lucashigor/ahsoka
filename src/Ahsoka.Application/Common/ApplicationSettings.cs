namespace Ahsoka.Application.Common;

public record ApplicationSettings
{
    public IdentityProvider? IdentityProvider { get; init; }
    public Cors? Cors { get; init; }
}

public record Cors
{
    public List<string>? AllowedOrigins { get; set; }
}

public record IdentityProvider
{
    public List<string>? Scopes { get; init; }
    public string? SecretKey { get; init; }
    public string? Authority { get; init; }
    public string? SwaggerClientId { get; init; }
    public string? PublicKeyJwt { get; set; }
}
