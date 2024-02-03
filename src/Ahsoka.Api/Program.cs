using Ahsoka.Api.Common.Middlewares;
using Ahsoka.Application.Administrations.Configurations.Commands;
using Ahsoka.Application.Administrations.Configurations.Queries;
using Ahsoka.Application.Common.Models;
using Ahsoka.Application.Dto.Administrations.Configurations.Requests;
using Ahsoka.Application.Dto.Common.ApplicationsErrors.Models;
using Ahsoka.Application.Dto.Common.Requests;
using Ahsoka.Application.Dto.Common.Responses;
using Ahsoka.Kernel.Extensions;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.AddOpenTelemetry();
builder.AddInfrastructureServices();
builder.AddApplicationExtensionServices();
builder.AddApiExtensionServices();

builder.Services.AddTransient<GlobalExceptionHandlerMiddleware>();
builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(opts =>
                opts.SerializerOptions.Converters.Add(new ErrorCodeConverter()));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

app.MapPost("/v1/configurations", async (RegisterConfigurationInput input,
    IMediator _mediator,
    CancellationToken cancellationToken ) =>
{
    if (input == null)
    {
        return Results.UnprocessableEntity(new DefaultResponse<object>());
    }
    
    var entity = input.Adapt<RegisterConfigurationCommand>();

    var output = await _mediator.Send(entity, cancellationToken);

    if(output is null)
    {    
        return Results.BadRequest(new DefaultResponse<object>());
    }
    
    return Results.Created($"/v1/configurations/{output.Id}",new DefaultResponse<object>(output));
})
.WithName("CreateConfiguration")
.WithOpenApi();

app.MapGet("/v1/configurations/list", async (
    [FromServices] IMediator mediator,
    CancellationToken cancellationToken,
    [FromQuery] int? page = null,
    [FromQuery(Name = "per_page")] int? perPage = null,
    [FromQuery] string? search = null,
    [FromQuery] string? sort = null,
    [FromQuery] SearchOrder? dir = null) =>
{
    var input = new ListConfigurationsQuery(
        page ?? 0,
        perPage ?? 10,
        search,
        sort,
        dir ?? SearchOrder.Asc
        );

    var output = await mediator.Send(input, cancellationToken);
    return Results.Ok(output);
})
.WithName("ListConfigurations")
.WithOpenApi();

app.MapGet("/v1/configurations/{id:guid}", async (
    [FromRoute] Guid id,
    [FromServices] IMediator mediator,
    [FromServices] Notifier notifier,
    CancellationToken cancellationToken) =>
{
    var output = await mediator.Send(new GetConfigurationByIdQuery(id), cancellationToken);

    return Results.Ok(output);
})
.WithName("GetConfigurationById")
.WithOpenApi();

app.MapDelete("/v1/configurations/{id:guid}", async (
    [FromRoute] Guid id,
    [FromServices] IMediator mediator,
    [FromServices] Notifier notifier,
    CancellationToken cancellationToken) =>
{
    var output = await mediator.Send(new GetConfigurationByIdQuery(id), cancellationToken);

    return Results.Ok(output);
})
.WithName("GetConfigurationById")
.WithOpenApi();

app.MapPrometheusScrapingEndpoint();

app.Run();
