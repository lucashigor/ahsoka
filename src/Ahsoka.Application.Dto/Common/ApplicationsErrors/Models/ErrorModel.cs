namespace Ahsoka.Application.Dto.Common.ApplicationsErrors.Models;

public sealed record ErrorModel
{
    public ApplicationErrorCode Code { get; init; }
    public string Message { get; init; }
    public string InnerMessage { get; private set; }

    public ErrorModel(ApplicationErrorCode code, string message)
    {
        Code = code;
        Message = message;
        InnerMessage = string.Empty;
    }

    public ErrorModel(ApplicationErrorCode code, string message, string innerMessage)
    {
        Code = code;
        Message = message;
        InnerMessage = innerMessage;
    }

    public ErrorModel ChangeInnerMessage(string message)
    {
        InnerMessage = message;

        return this;
    }
}
