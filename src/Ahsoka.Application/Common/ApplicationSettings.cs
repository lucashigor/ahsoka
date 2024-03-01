namespace Ahsoka.Application.Common;

public record ApplicationSettings
{
    public IdentityProvider? IdentityProvider { get; init; }
    public Cors? Cors { get; init; }
    public OpenTelemetryConfig? OpenTelemetryConfig { get; init; }
}

public record Cors(List<string>? AllowedOrigins);

public record IdentityProvider
{
    public List<string>? Scopes { get; init; }
    public string? SecretKey { get; init; }
    public string? Authority { get; init; }
    public string? SwaggerClientId { get; init; }
    public string? PublicKeyJwt { get; set; }
}

public record OpenTelemetryConfig(string? StatusGaugeName, string? DurationGaugeName, string? Endpoint);

public record RabbitMq(string? Host, string? Username, string? Password);
