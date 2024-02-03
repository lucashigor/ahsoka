using Ahsoka.Domain.Entities.Admin.Configurations.Errors;
using Ahsoka.Domain.Entities.Admin.Configurations.Events;
using Ahsoka.Domain.SeedWork;
using Ahsoka.Domain.Validation;

namespace Ahsoka.Domain.Entities.Admin.Configurations;

public record struct ConfigurationId(Guid Value)
{
    public static ConfigurationId New() => new(Guid.NewGuid());

    public static ConfigurationId Load(string value)
    {
        if (!Guid.TryParse(value, out Guid guid))
        {
            throw new ArgumentException("The value provided is not a valid GUID.", nameof(value));
        }
        return new ConfigurationId(guid);
    }

    public static ConfigurationId Load(Guid value) => new(value);

    public override readonly string ToString() => Value.ToString();

    public static implicit operator ConfigurationId(Guid value) => new(value);

    public static implicit operator Guid(ConfigurationId id) => id.Value;
}

public class Configuration : AggregateRoot<ConfigurationId>
{
    public string Name { get; private set; }
    public string Value { get; private set; }
    public string Description { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime? ExpireDate { get; private set; }
    public string CreatedBy { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public bool IsDeleted { get; private set; }

    public ConfigurationStatus Status
    {
        get
        {
            if (StartDate > DateTime.UtcNow)
            {
                return ConfigurationStatus.Awaiting;
            }

            if (ExpireDate.HasValue is false || ExpireDate.Value > DateTime.UtcNow)
            {
                return ConfigurationStatus.Active;
            }

            if (ExpireDate.HasValue && ExpireDate.Value < DateTime.UtcNow)
            {
                return ConfigurationStatus.Expired;
            }

            return ConfigurationStatus.Undefined;
        }
    }

    private Configuration()
    {
        Id = ConfigurationId.New();
        Name = string.Empty;
        Value = string.Empty;
        Description = string.Empty;
        CreatedBy = string.Empty;
        StartDate = DateTime.MinValue;
        CreatedAt = DateTime.MinValue;
        IsDeleted = false;
    }

    public static Configuration New(
        string name,
        string value,
        string description,
        DateTime startDate,
        DateTime? expireDate,
        string userId)
    {
        var entity = new Configuration();

        entity.SetValues(name, value, description, startDate, expireDate, userId, DateTime.UtcNow);

        entity.Validate();

        entity.RaiseDomainEvent(new ConfigurationCreatedDomainEvent(entity));

        return entity;
    }
    private void SetValues(
            string name,
            string value,
            string description,
            DateTime startDate,
            DateTime? expireDate,
            string userId,
            DateTime createdAt)
    {

        if (startDate < DateTimeOffset.UtcNow.AddSeconds(-5))
        {
            AddNotification($"{nameof(StartDate)} should be greater than now", ConfigurationsErrorsCodes.Validation);
        }

        if (expireDate.HasValue && ExpireDate < startDate)
        {
            AddNotification(DefaultsErrorsMessages.Date0CannotBeBeforeDate1.GetMessage(nameof(ExpireDate), nameof(StartDate)),
                ConfigurationsErrorsCodes.Validation);
        }

        if (expireDate.HasValue && expireDate < DateTimeOffset.UtcNow.AddSeconds(-5))
        {
            AddNotification($"{nameof(ExpireDate)} should be greater than now", ConfigurationsErrorsCodes.Validation);
        }

        AddNotification(startDate.NotDefaultDateTime());
        AddNotification(expireDate.NotDefaultDateTime());
        AddNotification(name.NotNullOrEmptyOrWhiteSpace());
        AddNotification(name.BetweenLength(3, 100));
        AddNotification(value.NotNullOrEmptyOrWhiteSpace());
        AddNotification(value.BetweenLength(1, 2500));
        AddNotification(description.NotNullOrEmptyOrWhiteSpace());
        AddNotification(description.BetweenLength(3, 1000));

        Name = name;
        Value = value;
        Description = description;
        StartDate = startDate;
        ExpireDate = expireDate;
        CreatedBy = userId;
        CreatedAt = createdAt;
    }

    public void Delete()
    {
        if (Status == ConfigurationStatus.Expired || Status == ConfigurationStatus.Active)
        {
            var message = "not allowed to delete expired or active configurations";
            AddNotification(new(nameof(ExpireDate), message, ConfigurationsErrorsCodes.ErrorOnDelete));
            return;
        }

        IsDeleted = true;

        RaiseDomainEvent(new ConfigurationDeletedDomainEvent(this));
    }

    public void Update(string name, string value, string description, DateTime startDate, DateTime? expireDate)
    {
        var StartDateHasChanges = StartDate.Equals(startDate) is false;
        var ExpireDateHasChanges = ExpireDate.Equals(expireDate) is false;
        var NameHasChanges = Name.Equals(name) is false;
        var ValueHasChanges = Value.Equals(value) is false;

        if (Status.Equals(ConfigurationStatus.Expired) &&
                (StartDateHasChanges
                || ExpireDateHasChanges
                || NameHasChanges
                || ValueHasChanges))
        {
            var message = "only description are allowed to change on expired configuration";
            AddNotification(new(nameof(ExpireDate), message, ConfigurationsErrorsCodes.OnlyDescriptionAllowedToChange));
            return;
        }

        if (Status.Equals(ConfigurationStatus.Active) &&
            (NameHasChanges
            || StartDateHasChanges
            || ValueHasChanges))
        {
            var message = "it is not allowed to change name on active configuration";
            AddNotification(new(nameof(ExpireDate), message, ConfigurationsErrorsCodes.ErrorOnChangeName));
            return;
        }

        SetValues(name, value, description, startDate, expireDate, CreatedBy, CreatedAt);

        RaiseDomainEvent(new ConfigurationUpdatedDomainEvent(this));
    }
}
