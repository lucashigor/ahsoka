namespace Ahsoka.Domain;
using System.Runtime.CompilerServices;

public static class DomainValidation
{
    public static Notification? NotNull(this object target,
                              [CallerArgumentExpression("target")] string fieldName = "")
    {
        Notification? notification = null;

        if (target is null)
        {
            var message = DefaultsErrorsMessages.NotNull.GetMessage(fieldName);
            notification = new Notification(fieldName, message, CommonErrorCodes.Validation);
        }

        return notification;
    }

    public static Notification? NotNull(this Guid target,
                              [CallerArgumentExpression("target")] string fieldName = "")
    {
        Notification? notification = null;

        if (target == Guid.Empty)
        {
            var message = DefaultsErrorsMessages.NotNull.GetMessage(fieldName);
            notification = new Notification(fieldName, message, CommonErrorCodes.Validation);
        }

        return notification;
    }

    public static Notification? NotNullOrEmptyOrWhiteSpace(this string target,
                              [CallerArgumentExpression("target")] string fieldName = "")
    {
        Notification? notification = null;

        if (string.IsNullOrWhiteSpace(target) || string.IsNullOrEmpty(target))
        {
            var message = DefaultsErrorsMessages.NotNull.GetMessage(fieldName);
            notification = new Notification(fieldName, message, CommonErrorCodes.Validation);
        }

        return notification;
    }

    public static Notification? NotDefaultDateTime(this DateTime target,
                              [CallerArgumentExpression("target")] string fieldName = "")
    {
        DateTime? nullableTarget = target;

        var notification = NotDefaultDateTime(nullableTarget, fieldName);

        return notification;
    }

    public static Notification? NotDefaultDateTime(this DateTime? target,
                          [CallerArgumentExpression("target")] string fieldName = "")
    {
        Notification? notification = null;
        
        if (target.HasValue && target.Value == default)
        {
            var message = DefaultsErrorsMessages.NotDefaultDateTime.GetMessage(fieldName);
            notification = new Notification(fieldName, message, CommonErrorCodes.Validation);
        }

        return notification;
    }

    public static Notification? BetweenLength(this string? target, int minLength, int maxLength,
                              [CallerArgumentExpression("target")] string fieldName = "")
    {
        Notification? notification = null;

        if (!string.IsNullOrEmpty(target) && (target.Length < minLength || target.Length > maxLength))
        {
            var message = DefaultsErrorsMessages.BetweenLength.GetMessage(fieldName, minLength, maxLength);
            notification = new Notification(fieldName, message, CommonErrorCodes.Validation);
        }

        return notification;
    }

    public static Notification? ValidUrl(this string target,
                              [CallerArgumentExpression("target")] string fieldName = "")
    {
        Notification? notification = null;

        if (!string.IsNullOrEmpty(target) && !Uri.TryCreate(target, UriKind.Absolute, out _))
        {
            var message = DefaultsErrorsMessages.InvalidUrl.GetMessage(fieldName);
            notification = new Notification(fieldName, message, CommonErrorCodes.Validation);
        }

        return notification;
    }
}