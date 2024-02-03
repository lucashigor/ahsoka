namespace Ahsoka.Domain.Common.ValuesObjects;

public sealed record Notification
{
    public DomainErrorCode Error { get; init; }
    public string FieldName { get; init; }
    public string Message { get; init; }

    public Notification(string fieldName, string message, DomainErrorCode error)
    {
        Error = error;
        FieldName = fieldName;
        Message = message;
    }
    public Notification(string message, DomainErrorCode error)
    {
        Error = error;
        FieldName = string.Empty;
        Message = message;
    }

    public override string ToString() => $"{Error.Value}: Message - {Message}";
}


public static class NotificationExtensions
{
    public static string GetMessage(this IList<Notification> list)
    {
        var ret = "";

        foreach (Notification notification in list)
        {
            if (ret.Length > 0)
            {
                ret += ";";
            }

            ret += notification.ToString();
        }

        return ret;
    }
}