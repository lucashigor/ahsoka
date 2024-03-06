using Ahsoka.Application.Dto.Common.ApplicationsErrors;
using Ahsoka.Application.Dto.Common.Responses;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Ahsoka.Api.Controllers;

public class BaseController() : ControllerBase
{
    protected IResult Result<T>(ApplicationResult<T> response) where T : class
    {
        var traceId = Activity.Current?.TraceId.ToString() ?? string.Empty;

        if (response.Data is null 
            && response.Warnings.Count == 0 
            && response.Errors.Count == 0
            && response.Infos.Count == 0)
        {
            return Results.NoContent();
        }

        DefaultResponse<T> responseDto = new DefaultResponse<T>()
            with
        {
            Data = response.Data,
            TraceId = traceId,
        };

        if (response.Warnings.Count != 0)
        {
            responseDto.Warnings.AddRange(response.Warnings);
        }

        if (response.Errors.Count != 0)
        {
            responseDto.Errors.AddRange(response.Errors);

            return Results.BadRequest(responseDto);
        }

        if (response.Infos.Count != 0)
        {
            responseDto.Errors.AddRange(response.Errors);

            return Results.BadRequest(responseDto);
        }

        return Results.Ok(responseDto);
    }

    protected ApplicationResult<T> CheckIdIfIdIsNull<T>(Guid? id) where T : class
    {
        var response = ApplicationResult<T>.Success();

        if (id == Guid.Empty)
        {
            var err = Errors.Validation();

            err.ChangeInnerMessage("Id cannot be null");

            response.AddError(err);
        }

        return response;
    }
}
