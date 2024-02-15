// Ignore Spelling: app

using Ahsoka.Api.Common.Swagger.OperationFilter;
using Ahsoka.Application.Common;
using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Ahsoka.Api.Common.Swagger;

public static class SwaggerExtension
{
    public static IServiceCollection AddSwagger(this IServiceCollection services, IConfiguration configuration)
    {
        var authOptions = configuration.GetSection("IdentityProvider").Get<IdentityProvider>() ?? new IdentityProvider();

        var scopes = authOptions.Scopes?.ToDictionary(scope => scope);

        services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,
                Flows = new OpenApiOAuthFlows
                {
                    AuthorizationCode = new OpenApiOAuthFlow
                    {
                        AuthorizationUrl = new Uri($"{authOptions.Authority}/protocol/openid-connect/auth"),
                        TokenUrl = new Uri($"{authOptions.Authority}/protocol/openid-connect/token"),
                        Scopes = scopes
                    }
                }
            });

            options.OperationFilter<AuthorizeCheckOperationFilter>();
            options.OperationFilter<SwaggerDefaultValuesFilter>();
        });


        services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerGenOptions>();

        AddApiVersioning(services);

        return services;
    }

    public static IApplicationBuilder UseCustomSwagger(this IApplicationBuilder app,
        IOptions<ApplicationSettings> configuration,
        IApiVersionDescriptionProvider versionDescription)
    {
        app.UseSwagger();
        app.UseSwaggerUI(o =>
        {
            foreach (var description in
                versionDescription.ApiVersionDescriptions.Select(x => x.GroupName))
            {
                o.SwaggerEndpoint($"/swagger/{description}/swagger.json",
                    $"Ahsoka - {description.ToUpper()}");

                o.RoutePrefix = string.Empty;

                o.OAuthClientId(configuration.Value.IdentityProvider?.SwaggerClientId);
                o.OAuthClientSecret(configuration.Value.IdentityProvider?.SecretKey);
                o.OAuthAppName("Ahsoka - Swagger");
                o.OAuthUsePkce();
            }
        });

        return app;
    }

    private static void AddApiVersioning(IServiceCollection services)
    {
        services.AddApiVersioning(o =>
        {
            o.AssumeDefaultVersionWhenUnspecified = true;
            o.DefaultApiVersion = new ApiVersion(1, 0);
        }).AddApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV";
            options.SubstituteApiVersionInUrl = true;
        });
    }
}
