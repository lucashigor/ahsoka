using Ahsoka.Application.Administrations.Configurations.Commands;
using Ahsoka.Domain.Common.ValuesObjects;
using Ahsoka.Domain.Entities.Admin.Configurations;
using Ahsoka.TestsUtil;
using System.Collections;

namespace Ahsoka.Unit.Tests.Domain.Entities.Admin.Configurations.Helpers;


public class TestInvalidDataGenerator : IEnumerable<object[]>
{
    /*
        string? Name,
        string? Value,
        string? Description,
        DateTime? StartDate,
        DateTime? ExpireDate,
        DomainErrorCode error, 
        string fieldName
     */
    public static IEnumerable<object[]> UpdateWithErrorOnExpiredConfig()
    {
        yield return new object[] { ConfigurationFixture.GetValidName(), null!, null!, null!, DomainErrorCode.OnlyDescriptionAllowedToChange, nameof(Configuration.ExpireDate) };
        yield return new object[] { null!, ConfigurationFixture.GetValidValue(), null!, null!, DomainErrorCode.OnlyDescriptionAllowedToChange, nameof(Configuration.ExpireDate) };
        yield return new object[] { null!, null!, ConfigurationFixture.GetValidStartDate(ConfigurationStatus.Awaiting), null!, DomainErrorCode.OnlyDescriptionAllowedToChange, nameof(Configuration.ExpireDate) };
        yield return new object[] { null!, null!, null!, ConfigurationFixture.GetValidExpireDate(ConfigurationStatus.Awaiting), DomainErrorCode.OnlyDescriptionAllowedToChange, nameof(Configuration.ExpireDate) };
    }

    public static IEnumerable<object[]> ValidConfigurationStatusTestData()
    {
        yield return new object[] { -2, -1, ConfigurationStatus.Expired };
        yield return new object[] { -2, null!, ConfigurationStatus.Active };
        yield return new object[] { -2, 1, ConfigurationStatus.Active };
        yield return new object[] { 1, 2, ConfigurationStatus.Awaiting };
    }

    public static IEnumerable<object[]> GetPersonFromDataGenerator()
    {
        yield return new object[]
        {
            new BaseConfiguration(
                Name: string.Empty,
                Value: ConfigurationFixture.GetValidValue(),
                Description: ConfigurationFixture.GetValidDescription(),
                StartDate: ConfigurationFixture.GetValidStartDate(ConfigurationStatus.Awaiting),
                ExpireDate: ConfigurationFixture.GetValidExpireDate(ConfigurationStatus.Awaiting)),
            DomainErrorCode.Validation,
            nameof(Configuration.Name)
        };

        yield return new object[]
        {
            new BaseConfiguration(
                Name:ConfigurationFixture.GetValidName(),
                Value: string.Empty,
                Description: ConfigurationFixture.GetValidDescription(),
                StartDate: ConfigurationFixture.GetValidStartDate(ConfigurationStatus.Awaiting),
                ExpireDate: ConfigurationFixture.GetValidExpireDate(ConfigurationStatus.Awaiting)),
            DomainErrorCode.Validation,
            nameof(Configuration.Value)
        };

        yield return new object[]
        {
            new BaseConfiguration(
                Name:ConfigurationFixture.GetValidName(),
                Value: ConfigurationFixture.GetValidValue(),
                Description: string.Empty,
                StartDate: ConfigurationFixture.GetValidStartDate(ConfigurationStatus.Awaiting),
                ExpireDate: ConfigurationFixture.GetValidExpireDate(ConfigurationStatus.Awaiting)),
            DomainErrorCode.Validation,
            nameof(Configuration.Description)
        };

        yield return new object[]
        {
            new BaseConfiguration(
                Name:ConfigurationFixture.GetValidName(),
                Value: ConfigurationFixture.GetValidValue(),
                Description: ConfigurationFixture.GetValidDescription(),
                StartDate: DateTime.UtcNow.AddSeconds(-10),
                ExpireDate: ConfigurationFixture.GetValidExpireDate(ConfigurationStatus.Awaiting)),
            DomainErrorCode.Validation,
            nameof(Configuration.StartDate)
        };

        yield return new object[]
        {
            new BaseConfiguration(
                Name:ConfigurationFixture.GetValidName(),
                Value: ConfigurationFixture.GetValidValue(),
                Description: ConfigurationFixture.GetValidDescription(),
                StartDate: ConfigurationFixture.GetValidStartDate(ConfigurationStatus.Awaiting),
                ExpireDate: ConfigurationFixture.GetValidStartDate(ConfigurationStatus.Awaiting).AddSeconds(-10)),
            DomainErrorCode.Validation,
            nameof(Configuration.ExpireDate)
        };

        yield return new object[]
        {
            new BaseConfiguration(
                Name:ConfigurationFixture.GetValidName(),
                Value: ConfigurationFixture.GetValidValue(),
                Description: ConfigurationFixture.GetValidDescription(),
                StartDate: ConfigurationFixture.GetValidStartDate(ConfigurationStatus.Awaiting),
                ExpireDate: DateTime.UtcNow.AddSeconds(-10)),
            DomainErrorCode.Validation,
            nameof(Configuration.ExpireDate)
        };
    }

    public IEnumerator<object[]> GetEnumerator() => (IEnumerator<object[]>)GetPersonFromDataGenerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}