using Ahsoka.Domain.Common;
using Ahsoka.Domain.Common.ValuesObjects;
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
public record ConfigurationStatus : Enumeration<int>
{
    private ConfigurationStatus(int id, string name) : base(id, name)
    {
    }

    public static readonly ConfigurationStatus Undefined = new(0, nameof(Undefined));
    public static readonly ConfigurationStatus Awaiting = new(1, nameof(Awaiting));
    public static readonly ConfigurationStatus Active = new(2, nameof(Active));
    public static readonly ConfigurationStatus Expired = new(3, nameof(Expired));
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
            if (IsDeleted)
            {
                return ConfigurationStatus.Undefined;
            }

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

    public static (Result, Configuration?) New(
        string name,
        string value,
        string description,
        DateTime startDate,
        DateTime? expireDate,
        string userId)
    {
        var entity = new Configuration();

        if (startDate < DateTimeOffset.UtcNow.AddSeconds(-5))
        {
            entity.AddNotification(nameof(StartDate), $"{nameof(StartDate)} should be greater than now", DomainErrorCode.Validation);
        }

        if (expireDate.HasValue && expireDate < startDate)
        {
            entity.AddNotification(nameof(ExpireDate), DefaultsErrorsMessages.Date0CannotBeBeforeDate1.GetMessage(nameof(ExpireDate), nameof(StartDate)),
                DomainErrorCode.Validation);
        }

        if (expireDate.HasValue && expireDate < DateTimeOffset.UtcNow.AddSeconds(-5))
        {
            entity.AddNotification(nameof(ExpireDate), $"{nameof(ExpireDate)} should be greater than now", DomainErrorCode.Validation);
        }

        entity.SetValues(name, value, description, startDate, expireDate, userId, DateTime.UtcNow);

        var result = entity.Validate();

        if (result.IsFailure)
        {
            return (result, null);
        }

        entity.RaiseDomainEvent(new ConfigurationCreatedDomainEvent(entity));

        return (result, entity);
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
        AddNotification(startDate.NotDefaultDateTime());
        AddNotification(expireDate.NotDefaultDateTime());
        AddNotification(name.NotNullOrEmptyOrWhiteSpace());
        AddNotification(name.BetweenLength(3, 100));
        AddNotification(value.NotNullOrEmptyOrWhiteSpace());
        AddNotification(value.BetweenLength(1, 2500));
        AddNotification(description.NotNullOrEmptyOrWhiteSpace());
        AddNotification(description.BetweenLength(3, 1000));

        if (Notifications.Count != 0)
        {
            return;
        }

        Name = name;
        Value = value;
        Description = description;
        StartDate = startDate;
        ExpireDate = expireDate;
        CreatedBy = userId;
        CreatedAt = createdAt;
    }

    #region Update
    public Result Update(string name, string value, string description, DateTime startDate, DateTime? expireDate)
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
            AddNotification(new(nameof(ExpireDate), message, DomainErrorCode.OnlyDescriptionAllowedToChange));

            return Validate();
        }

        if (Status.Equals(ConfigurationStatus.Active) &&
            (NameHasChanges
            || StartDateHasChanges
            || ValueHasChanges))
        {
            var message = "it is not allowed to change name on active configuration";
            AddNotification(new(nameof(StartDate), message, DomainErrorCode.ErrorOnChangeName));

            return Validate();
        }

        SetValues(name, value, description, startDate, expireDate, CreatedBy, CreatedAt);

        var result = Validate();

        if (result.IsFailure)
        {
            return result;
        }

        RaiseDomainEvent(new ConfigurationUpdatedDomainEvent(this));

        return result;
    }

    #endregion

    public Result Delete()
    {
        if (Status == ConfigurationStatus.Expired)
        {
            AddNotification(nameof(ExpireDate),
                "not allowed to delete expired configurations",
                DomainErrorCode.ErrorOnDelete);
        }

        if (Status == ConfigurationStatus.Active)
        {
            Update(Name, Value, Description, StartDate, DateTime.UtcNow);
            AddWarning(nameof(ExpireDate),
                "expire date set to today",
                DomainErrorCode.SetExpireDateToToday);
        }

        if (Status == ConfigurationStatus.Awaiting)
        {
            IsDeleted = true;

            RaiseDomainEvent(new ConfigurationDeletedDomainEvent(this));
        }

        var result = Validate();

        return result;
    }
}
