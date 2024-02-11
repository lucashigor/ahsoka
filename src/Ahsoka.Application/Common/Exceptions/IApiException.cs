using Ahsoka.Application.Dto.Common.ApplicationsErrors.Models;
using System.Net;

namespace Ahsoka.Application.Common.Exceptions;

public interface IApiException
{
    List<ErrorModel> Errors { get; }
    HttpStatusCode Status { get; }
}
