using Ahsoka.Application.Administrations.Configurations.Commands;
using Ahsoka.Domain.Entities.Admin.Configurations;

namespace Ahsoka.TestsUtil;

public static class ConfigurationFixture
{
    public static Configuration LoadConfiguration(ConfigurationStatus configurationStatus)
        => LoadConfiguration(GetValidBaseConfiguration(configurationStatus), Guid.NewGuid());

    public static Configuration LoadConfiguration(Guid userId, ConfigurationStatus configurationStatus)
        => LoadConfiguration(GetValidBaseConfiguration(configurationStatus), userId);

    public static Configuration LoadConfiguration(BaseConfiguration @base)
        => LoadConfiguration(@base, Guid.NewGuid());

    public static Configuration LoadConfiguration(BaseConfiguration @base, Guid userId)
    {
        var propertyValues = new Dictionary<string, object>
        {
            { nameof(Configuration.Name), @base.Name },
            { nameof(Configuration.Value), @base.Value },
            { nameof(Configuration.Description), @base.Description },
            { nameof(Configuration.CreatedBy), userId.ToString() },
            { nameof(Configuration.StartDate), @base.StartDate },
            { nameof(Configuration.CreatedAt), @base.StartDate }
         };

        if (@base.ExpireDate != null)
        {
            propertyValues.Add(nameof(Configuration.ExpireDate), @base.ExpireDate);
        }

        return GeneralFixture.CreateInstanceAndSetProperties<Configuration>(propertyValues);
    }

    public static Configuration GetValidConfiguration()
        => GetValidConfiguration(Guid.NewGuid(), ConfigurationStatus.Awaiting);

    public static Configuration GetValidConfiguration(ConfigurationStatus configurationStatus)
        => GetValidConfiguration(Guid.NewGuid(), configurationStatus);

    public static Configuration GetValidConfiguration(Guid userId)
        => GetValidConfiguration(userId, ConfigurationStatus.Awaiting);

    public static Configuration GetValidConfiguration(Guid userId, ConfigurationStatus configurationStatus)
    {
        return LoadConfiguration(
            new BaseConfiguration(
            Name: GetValidName(),
            Value: GetValidValue(),
            Description: GetValidDescription(),
            StartDate: GetValidStartDate(configurationStatus),
            ExpireDate: GetValidExpireDate(configurationStatus)), userId);
    }

    public static string GetValidName()
        => GeneralFixture.GetStringRightSize(3, 100);

    public static string GetValidValue()
        => GeneralFixture.GetStringRightSize(1, 2500);

    public static string GetValidDescription()
        => GeneralFixture.GetStringRightSize(3, 1000);

    public static DateTime GetValidStartDate(ConfigurationStatus configuration)
    {
        DateTime currentTime = DateTime.UtcNow;

        if (configuration == ConfigurationStatus.Awaiting)
        {
            return currentTime.AddDays(1);
        }
        else if (configuration == ConfigurationStatus.Active || configuration == ConfigurationStatus.Expired)
        {
            return currentTime.AddMonths(-2);
        }
        else
        {
            throw new ArgumentOutOfRangeException();
        }
    }

    public static DateTime GetValidExpireDate(ConfigurationStatus configuration)
    {
        DateTime currentTime = DateTime.UtcNow;

        if (configuration == ConfigurationStatus.Awaiting || configuration == ConfigurationStatus.Active)
        {
            return currentTime.AddMonths(1);
        }
        else if (configuration == ConfigurationStatus.Expired)
        {
            return currentTime.AddMonths(-1);
        }
        else
        {
            throw new ArgumentOutOfRangeException();
        }
    }

    private static BaseConfiguration GetValidBaseConfiguration(ConfigurationStatus configurationStatus)
    {
        return new BaseConfiguration(
                        Name: GetValidName(),
                        Value: GetValidValue(),
                        Description: GetValidDescription(),
                        StartDate: GetValidStartDate(configurationStatus),
                        ExpireDate: GetValidExpireDate(configurationStatus)
                    );
    }

}
