using System.Net;
using Ahsoka.Application.Dto;

namespace Ahsoka.Application;

public interface IApiException
{
    List<ErrorModel> Errors { get; }
    HttpStatusCode Status { get; }
}
