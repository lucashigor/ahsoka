using Ahsoka.Api.Common.Middlewares;
using Ahsoka.Api.Common.Swagger;
using Ahsoka.Application.Common;
using Ahsoka.Application.Dto.Common.ApplicationsErrors.Models;
using Ahsoka.Kernel.Extensions;
using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.AddOpenTelemetry();
builder.AddInfrastructureServices();
builder.AddApplicationExtensionServices();
builder.AddApiExtensionServices();

builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(opts =>
                opts.SerializerOptions.Converters.Add(new ErrorCodeConverter()));
builder.Services.AddTransient<GlobalExceptionHandlerMiddleware>();

builder.Services
    .ConfigureJWT(builder.Configuration);

builder.Services.AddSwagger(builder.Configuration);
builder.Services.Configure<ApplicationSettings>(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
if (app.Environment.IsDevelopment())
{
    app.UseSwaggerUI();
}

var configs = app.Services.GetRequiredService<IOptions<ApplicationSettings>>();

app.UseCustomSwagger(configs,
    app.Services.GetRequiredService<IApiVersionDescriptionProvider>());

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapPrometheusScrapingEndpoint();

app.Run();