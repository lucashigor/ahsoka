namespace Ahsoka.Application.Common.Models;

public sealed record ValidationConstant
{
    public static readonly string InvalidParameter = "{PropertyName} invalid";
    public static readonly string RequiredParameter = "{PropertyName} is a mandatory parameter";
    public static readonly string RequiredField = "{PropertyName} is a mandatory field";
    public static readonly string MaxLengthExceeded = "Number of characters allowed{MaxLength} was exceeded for the field {PropertyName}";
    public static readonly string LengthError = "Number of characters allowed are between {MinLength} and {MaxLength} for the field {PropertyName}";
    public static readonly string WrongEmail = "Wrong Email";
}
