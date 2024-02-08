using Ahsoka.Api.Common.Middlewares;
using Ahsoka.Application.Administrations.Configurations.Commands.ChangeConfiguration;
using Ahsoka.Application.Administrations.Configurations.Commands.ModifyConfiguration;
using Ahsoka.Application.Administrations.Configurations.Commands.RegisterConfiguration;
using Ahsoka.Application.Administrations.Configurations.Commands.RemoveConfiguration;
using Ahsoka.Application.Administrations.Configurations.Queries;
using Ahsoka.Application.Common.Extensions;
using Ahsoka.Application.Common.Models;
using Ahsoka.Application.Dto.Administrations.Configurations.Requests;
using Ahsoka.Application.Dto.Common.ApplicationsErrors.Models;
using Ahsoka.Application.Dto.Common.Requests;
using Ahsoka.Application.Dto.Common.Responses;
using Ahsoka.Kernel.Extensions;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using _application = Ahsoka.Application.Administrations.Configurations.Commands;

var builder = WebApplication.CreateBuilder(args);

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

app.MapPost("/v1/configurations", async (BaseConfiguration input,
    IMediator _mediator,
    CancellationToken cancellationToken ) =>
{
    if (input == null)
    {
        return Results.UnprocessableEntity(new DefaultResponse<object>());
    }
    
    var command = new RegisterConfigurationCommand(input);

    var output = await _mediator.Send(command, cancellationToken);

    if(output is null)
    {    
        return Results.BadRequest(new DefaultResponse<object>());
    }
    
    return Results.Created($"/v1/configurations/{output.Id}",new DefaultResponse<object>(output));
})
.WithName("CreateConfiguration")
.WithOpenApi();

app.MapGet("/v1/configurations", async (
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
    await mediator.Send(new RemoveConfigurationCommand(id), cancellationToken);

    return Results.NoContent();
})
.WithName("DeleteConfiguration")
.WithOpenApi();

app.MapPut("/v1/configurations/{id:guid}", async (
    [FromRoute] Guid id,
    [FromBody] BaseConfiguration request,
    [FromServices] IMediator mediator,
    [FromServices] Notifier notifier,
    CancellationToken cancellationToken) =>
{
    var command = new ChangeConfigurationCommand(id, request);

    var output = await mediator.Send(command, cancellationToken); ;

    return Results.Ok(output);
})
.WithName("UpdateConfiguration")
.WithOpenApi();

app.MapPatch("/v1/configurations/{id:guid}", async (
    [FromRoute] Guid id,
    [FromBody] JsonPatchDocument<BaseConfiguration> request,
    [FromServices] IMediator mediator,
    [FromServices] Notifier notifier,
    CancellationToken cancellationToken) =>
{
    var input = request.MapPatchInputToPatchCommand<BaseConfiguration, _application.BaseConfiguration>();

    var command = new ModifyConfigurationCommand(id, input);

    var output = await mediator.Send(command, cancellationToken);

    return Results.Ok(output);
})
.WithName("PatchConfiguration")
.WithOpenApi();

app.MapPrometheusScrapingEndpoint();

app.Run();
