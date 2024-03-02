using Ahsoka.Application.Administrations.Configurations.Commands.ChangeConfiguration;
using Ahsoka.Application.Administrations.Configurations.Commands.ModifyConfiguration;
using Ahsoka.Application.Administrations.Configurations.Commands.RegisterConfiguration;
using Ahsoka.Application.Administrations.Configurations.Commands.RemoveConfiguration;
using Ahsoka.Application.Administrations.Configurations.Queries;
using Ahsoka.Application.Common.Models;
using Ahsoka.Application.Dto.Administrations.Configurations.Requests;
using Ahsoka.Application.Dto.Administrations.Configurations.Responses;
using Ahsoka.Application.Dto.Common.Requests;
using Ahsoka.Application.Dto.Common.Responses;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace Ahsoka.Api.Controllers.Administrations.Configurations.v1;

[ApiController]
[ApiVersion("1.0")]
[Authorize(AuthenticationSchemes = "Bearer")]
[Route("v{version:apiVersion}/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[Consumes(MediaTypeNames.Application.Json)]
public class ConfigurationsController(Notifier notifier, IMediator mediator) : BaseController(notifier)
{
    private readonly IMediator _mediator = mediator;

    [HttpPost]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(DefaultResponse<ConfigurationOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IResult> Create(
        [FromBody] BaseConfiguration request,
        CancellationToken cancellationToken
    )
    {
        if (request == null)
        {
            return Results.UnprocessableEntity();
        }

        var command = new RegisterConfigurationCommand(request);

        var output = await _mediator.Send(command, cancellationToken);

        return Result(output);
    }

    [HttpPatch("{id:guid}")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(DefaultResponse<ConfigurationOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IResult> PatchConfiguration(
        [FromBody] JsonPatchDocument<BaseConfiguration> request,
        [FromRoute] Guid id,
        CancellationToken cancellationToken
    )
    {
        if (request == null)
        {
            return Results.UnprocessableEntity();
        }

        CheckIdIfIdIsNull(id);

        if (notifier.Errors.Any())
        {
            return Result<ConfigurationOutput>(null!);
        }

        var command = new ModifyConfigurationCommand(id, request);

        var output = await _mediator.Send(command, cancellationToken);

        return Result(output);
    }

    [HttpPut("{id:guid}")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(DefaultResponse<ConfigurationOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IResult> Update(
        [FromBody] BaseConfiguration request,
        [FromRoute] Guid id,
        CancellationToken cancellationToken
    )
    {
        CheckIdIfIdIsNull(id);

        if (notifier.Errors.Any())
        {
            return Result<ConfigurationOutput>(null!);
        }

        var command = new ChangeConfigurationCommand(id, request);

        var output = await _mediator.Send(command, cancellationToken);

        return Result(output);
    }

    [HttpDelete("{id:guid}")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IResult> Delete(
        [FromRoute] Guid id,
        CancellationToken cancellationToken
    )
    {
        CheckIdIfIdIsNull(id);

        if (notifier.Errors.Any())
        {
            return Result<ConfigurationOutput>(null!);
        }

        await _mediator.Send(new RemoveConfigurationCommand(id), cancellationToken);

        return Result<object>();
    }

    [HttpGet("{id:guid}")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(DefaultResponse<ConfigurationOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(DefaultResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IResult> GetById(
        [FromRoute] Guid id,
        CancellationToken cancellationToken
    )
    {
        CheckIdIfIdIsNull(id);

        if (notifier.Errors.Any())
        {
            return Result<ConfigurationOutput>(null!);
        }

        var output = await _mediator.Send(new GetConfigurationByIdQuery(id), cancellationToken);

        return Result(output);
    }

    [HttpGet]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(DefaultResponse<ListConfigurationsOutput>), StatusCodes.Status200OK)]
    public async Task<IResult> List(
        CancellationToken cancellationToken,
        [FromQuery] int? page = null,
        [FromQuery(Name = "per_page")] int? perPage = null,
        [FromQuery] string? search = null,
        [FromQuery] string? sort = null,
        [FromQuery] SearchOrder? dir = null
    )
    {
        var input = new ListConfigurationsQuery(
            page ?? 0,
            perPage ?? 10,
            search,
            sort,
            dir ?? SearchOrder.Asc
            );

        var output = await _mediator.Send(input, cancellationToken);
        return Result(output);
    }
}
