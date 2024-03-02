using Ahsoka.Application.Common.Models;
using Ahsoka.Application.Dto.Common.ApplicationsErrors;
using Ahsoka.Application.Dto.Common.Responses;
using Microsoft.AspNetCore.Mvc;

namespace Ahsoka.Api.Controllers;

public class BaseController(Notifier notifier) : ControllerBase
{
    protected readonly Notifier notifier = notifier;

    protected IResult Result<T>(T? model = null) where T : class
    {
        if (model is null && notifier.Warnings.Count == 0 && notifier.Errors.Count == 0)
        {
            return Results.NoContent();
        }

        DefaultResponse<T> responseDto = new(model!);

        if (notifier.Warnings.Any())
        {
            responseDto.Warnings.AddRange(notifier.Warnings);
        }

        if (notifier.Errors.Any())
        {
            responseDto.Errors.AddRange(notifier.Errors);

            return Results.BadRequest(responseDto);
        }

        return Results.Ok(responseDto);
    }

    protected void CheckIdIfIdIsNull(Guid id)
    {
        if (id == Guid.Empty)
        {
            var err = Errors.Validation();

            err.ChangeInnerMessage("Id cannot be null");

            this.notifier.Errors.Add(err);

        }
    }
}
