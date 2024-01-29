using System.Net.Http.Headers;

namespace Ahsoka.Domain;

public class Configuration : AggregateRoot
{
    public string Name { get; private set; }
    public string Value { get; private set; }
    public string Description { get; private set; }
    public DateTimeOffset StartDate { get; private set; }
    public DateTimeOffset? FinalDate { get; private set; }
    public string CreatedBy { get; init; }
    public DateTimeOffset CreatedAt { get; init; }

    public ConfigurationStatus GetStatus()
    {
        if(StartDate > DateTime.UtcNow)
        {
            return ConfigurationStatus.Previous;
        }

        if(FinalDate.HasValue is false || (FinalDate.Value > DateTime.UtcNow))
        {
            return ConfigurationStatus.Active;
        }

        if(FinalDate.HasValue && (FinalDate.Value < DateTime.UtcNow))
        {
            return ConfigurationStatus.Disable;
        }

        return ConfigurationStatus.Undefined;
    }

    private Configuration(Guid id,
        string name,
        string value,
        string description,
        DateTimeOffset startDate,
        DateTimeOffset? finalDate,
        string createdBy,
        DateTimeOffset createdAt)
    {
        Id = id;
        Name = name;
        Value = value;
        Description = description;
        StartDate = startDate;
        FinalDate = finalDate;
        CreatedBy = createdBy;
        CreatedAt = createdAt;

        Validate();
    }

    public static Configuration New(
        string name,
        string value,
        string description,
        DateTimeOffset startDate,
        DateTimeOffset? finalDate,
        string UserId)
    {
        var entity = new Configuration(Guid.NewGuid(), name, value, description, startDate, finalDate, UserId, DateTime.UtcNow);

        if (entity.StartDate < DateTimeOffset.UtcNow)
        {
            var message = "StartDate should be greater than now";

            var notification = new Notification(nameof(FinalDate), message, ConfigurationsErrorsCodes.Validation);

            entity.AddNotification(notification);
        }

        if (entity.FinalDate != (default) && finalDate < DateTimeOffset.UtcNow)
        {
            var message = "FinalDate should be greater than now";

            var notification = new Notification(nameof(FinalDate), message, ConfigurationsErrorsCodes.Validation);

            entity.AddNotification(notification);
        }

        entity.Validate();

        entity.RaiseDomainEvent(new ConfigurationCreatedDomainEvent(entity));

        return entity;
    }

    protected override void Validate()
    {
        AddNotification(Name.NotNullOrEmptyOrWhiteSpace());
        AddNotification(Name.BetweenLength(3, 100));

        AddNotification(Value.NotNullOrEmptyOrWhiteSpace());
        AddNotification(Value.BetweenLength(1, 2500));

        AddNotification(Description.NotNullOrEmptyOrWhiteSpace());
        AddNotification(Description.BetweenLength(3, 1000));

        AddNotification(StartDate.NotDefaultDateTime());

        AddNotification(FinalDate.NotDefaultDateTime());

        if (FinalDate != (default) && FinalDate < StartDate)
        {
            var message = DefaultsErrorsMessages.Date0CannotBeBeforeDate1.GetMessage(nameof(FinalDate), nameof(StartDate));

            var notification = new Notification(nameof(FinalDate), message, ConfigurationsErrorsCodes.Validation);

            AddNotification(notification);
        }

        base.Validate();
    }

    public void ChangeConfiguration(string name,
        string value,
        string description,
        DateTimeOffset startDate,
        DateTimeOffset? finalDate)
    {
        Name = name;
        Value = value;
        Description = description;
        StartDate = startDate;
        FinalDate = finalDate;

        Validate();
    }

    public void SetFinalDateToNow()
    {
        if (FinalDate > DateTimeOffset.UtcNow)
        {
            FinalDate = DateTimeOffset.UtcNow;
        }

        Validate();
    }
}
