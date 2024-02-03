using Ahsoka.Application.Dto.Common.ApplicationsErrors.Models;

namespace Ahsoka.Application.Dto.Common.ApplicationsErrors;

internal sealed record GenericErrorCodes : ErrorCode
{
    private GenericErrorCodes(int original) : base(original)
    {
    }

    public static readonly GenericErrorCodes Generic = new(10_000);
    public static readonly GenericErrorCodes UnavailableFeatureFlag = new(10_001);
    public static readonly GenericErrorCodes ClientHttp = new(10_002);
    public static readonly GenericErrorCodes Validation = new(10_003);
    public static readonly GenericErrorCodes InvalidOperationOnPatch = new(10_004);
    public static readonly GenericErrorCodes InvalidPathOnPatch = new(10_005);
    public static readonly GenericErrorCodes NotificationValuesError = new(10_006);
    public static readonly GenericErrorCodes DataBaseError = new(10_007);
}

public partial record Errors
{
    public static ErrorModel Generic() => new(GenericErrorCodes.Generic, "Unfortunately an error occurred during the processing.");
    public static ErrorModel GenericDataBaseError() => new(GenericErrorCodes.DataBaseError, "Unfortunately an error occurred during the processing.");
    public static ErrorModel UnavailableFeatureFlag() => new(GenericErrorCodes.UnavailableFeatureFlag, "Unavailable FeatureFlag.");
    public static ErrorModel ClientHttp() => new(GenericErrorCodes.ClientHttp, "Client Http error.");
    public static ErrorModel Validation() => new(GenericErrorCodes.Validation, "Unfortunately your request do not pass in our validation process.");
    public static ErrorModel InvalidOperationOnPatch() => new(GenericErrorCodes.InvalidOperationOnPatch, "This operation are not valid on patch.");
    public static ErrorModel InvalidPathOnPatch() => new(GenericErrorCodes.InvalidPathOnPatch, "This path cannot be changed on patch.");
    public static ErrorModel NotificationValuesError() => new(GenericErrorCodes.NotificationValuesError, "Error on creating a notification.");
}