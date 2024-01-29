namespace Ahsoka.Domain;

public static class DefaultsErrorsMessages
{
    public static readonly string NotNull = "The field {0} is required.";
    public static readonly string NotDefaultDateTime = "The date field {0} has to have valid data.";
    public static readonly string BetweenLength = "The field {0} has to have length between {1} and {2}.";
    public static readonly string Date0CannotBeBeforeDate1 = "{0} has to be greater than {1}.";
    public static readonly string InvalidUrl = "The field {0} has to have a valid Url.";

    /// <summary>
    /// ErrorsMessages.BetweenLength.GetMessage(nameof(Name),3,100)}
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="par"></param>
    /// <returns></returns>
    public static string GetMessage(this string msg, params object[] par)
    {
        return string.Format(msg, par);
    }
}
