using Ahsoka.Domain.Entities.Admin.Configurations;

namespace Ahsoka.TestsUtil;

public static class ConfigurationFixture
{
    public static Configuration GetValidConfiguration(Guid? userId)
    {
        return (Configuration.New(
            name: GetValidName(),
            value: GetValidValue(),
            description: GetValidDescription(),
            startDate: GetValidStartDate(),
            expireDate: GetValidExpireDate(),
            userId: userId.ToString() ?? Guid.NewGuid().ToString())).Item2!;
    }

    public static string GetValidName()
        => GeneralFixture.GetStringRightSize(3, 100);

    public static string GetValidValue()
        => GeneralFixture.GetStringRightSize(1, 2500);

    public static string GetValidDescription()
        => GeneralFixture.GetStringRightSize(3, 1000);

    public static DateTime GetValidStartDate()
        => DateTime.UtcNow.AddHours(1);

    public static DateTime GetValidExpireDate()
        => DateTime.UtcNow.AddDays(1);
}
