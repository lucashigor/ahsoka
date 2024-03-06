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
public record ConfigurationState : Enumeration<int>
{
    private ConfigurationState(int id, string name) : base(id, name)
    {
    }

    public static readonly ConfigurationState Undefined = new(0, nameof(Undefined));
    public static readonly ConfigurationState Awaiting = new(1, nameof(Awaiting));
    public static readonly ConfigurationState Active = new(2, nameof(Active));
    public static readonly ConfigurationState Expired = new(3, nameof(Expired));
}

public class Configuration : AggregateRoot<ConfigurationId>, ISoftDeletableEntity
{
    public string Name { get; private set; }
    public string Value { get; private set; }
    public string Description { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime? ExpireDate { get; private set; }
    public string CreatedBy { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public bool IsDeleted { get; private set; }
    public ConfigurationState State => GetStatus(IsDeleted, StartDate, ExpireDate);

    public static ConfigurationState GetStatus(bool _isDeleted, DateTime _startDate, DateTime? _expireDate)
    {
        if (_isDeleted)
        {
            return ConfigurationState.Undefined;
        }

        if (_startDate > DateTime.UtcNow)
        {
            return ConfigurationState.Awaiting;
        }

        if (_expireDate.HasValue is false || _expireDate.Value > DateTime.UtcNow)
        {
            return ConfigurationState.Active;
        }

        if (_expireDate.HasValue && _expireDate.Value < DateTime.UtcNow)
        {
            return ConfigurationState.Expired;
        }

        return ConfigurationState.Undefined;
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

    public static (DomainResult, Configuration?) New(
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

        entity.RaiseDomainEvent(ConfigurationCreatedDomainEvent.FromConfiguration(entity));

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
    public DomainResult Update(string name, string value, string description, DateTime startDate, DateTime? expireDate)
    {
        var StartDateHasChanges = StartDate.Equals(startDate) is false;
        var ExpireDateHasChanges = ExpireDate.Equals(expireDate) is false;
        var NameHasChanges = Name.Equals(name) is false;
        var ValueHasChanges = Value.Equals(value) is false;

        if (State.Equals(ConfigurationState.Expired) &&
                (StartDateHasChanges
                || ExpireDateHasChanges
                || NameHasChanges
                || ValueHasChanges))
        {
            var message = "only description are allowed to change on expired configuration";
            AddNotification(new(nameof(ExpireDate), message, DomainErrorCode.OnlyDescriptionAllowedToChange));

            return Validate();
        }

        if (State.Equals(ConfigurationState.Active) &&
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

        RaiseDomainEvent(ConfigurationUpdatedDomainEvent.FromConfiguration(this));

        return result;
    }

    #endregion

    public DomainResult Delete()
    {
        if (State == ConfigurationState.Expired)
        {
            AddNotification(nameof(ExpireDate),
                "not allowed to delete expired configurations",
                DomainErrorCode.ErrorOnDelete);
        }

        if (State == ConfigurationState.Active)
        {
            Update(Name, Value, Description, StartDate, DateTime.UtcNow);
            AddWarning(nameof(ExpireDate),
                "expire date set to today",
                DomainErrorCode.SetExpireDateToToday);
        }

        if (State == ConfigurationState.Awaiting)
        {
            IsDeleted = true;

            RaiseDomainEvent(ConfigurationDeletedDomainEvent.FromConfiguration(this));
        }

        return Validate();
    }
}
