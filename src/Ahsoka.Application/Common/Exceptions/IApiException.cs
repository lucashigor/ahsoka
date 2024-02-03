using System.Net;
using Ahsoka.Application.Dto.Common.ApplicationsErrors.Models;

namespace Ahsoka.Application.Common.Exceptions;

public interface IApiException
{
    List<ErrorModel> Errors { get; }
    HttpStatusCode Status { get; }
}
