using Ahsoka.Application.Administrations.Configurations.Commands;
using Ahsoka.Domain.Common.ValuesObjects;
using Ahsoka.Domain.Entities.Admin.Configurations;
using Ahsoka.TestsUtil;
using System.Collections;

namespace Ahsoka.Unit.Tests.Domain.Entities.Admin.Configurations.Helpers;


public class TestInvalidDataGenerator : IEnumerable<object[]>
{
    public static IEnumerable<object[]> GetPersonFromDataGenerator()
    {
        yield return new object[]
    {
new BaseConfiguration(
    Name: string.Empty,
    Value: ConfigurationFixture.GetValidValue(),
    Description: ConfigurationFixture.GetValidDescription(),
    StartDate: ConfigurationFixture.GetValidStartDate(),
    ExpireDate: ConfigurationFixture.GetValidExpireDate()),
DomainErrorCode.Validation,
nameof(Configuration.Name)
    };

        yield return new object[]
        {
new BaseConfiguration(
    Name:ConfigurationFixture.GetValidName(),
    Value: string.Empty,
    Description: ConfigurationFixture.GetValidDescription(),
    StartDate: ConfigurationFixture.GetValidStartDate(),
    ExpireDate: ConfigurationFixture.GetValidExpireDate()),
DomainErrorCode.Validation,
nameof(Configuration.Value)
        };

        yield return new object[]
        {
new BaseConfiguration(
    Name:ConfigurationFixture.GetValidName(),
    Value: ConfigurationFixture.GetValidValue(),
    Description: string.Empty,
    StartDate: ConfigurationFixture.GetValidStartDate(),
    ExpireDate: ConfigurationFixture.GetValidExpireDate()),
DomainErrorCode.Validation,
nameof(Configuration.Description)
        };

        yield return new object[]
        {
new BaseConfiguration(
    Name:ConfigurationFixture.GetValidName(),
    Value: ConfigurationFixture.GetValidValue(),
    Description: ConfigurationFixture.GetValidDescription(),
    StartDate: DateTime.UtcNow.AddSeconds(-10), // StartDate is less than now
    ExpireDate: ConfigurationFixture.GetValidExpireDate()),
DomainErrorCode.Validation,
nameof(Configuration.StartDate)
        };

        yield return new object[]
        {
new BaseConfiguration(
    Name:ConfigurationFixture.GetValidName(),
    Value: ConfigurationFixture.GetValidValue(),
    Description: ConfigurationFixture.GetValidDescription(),
    StartDate: ConfigurationFixture.GetValidStartDate(),
    ExpireDate: ConfigurationFixture.GetValidStartDate().AddSeconds(-10)), // ExpireDate is less than StartDate
DomainErrorCode.Validation,
nameof(Configuration.ExpireDate)
        };

        yield return new object[]
        {
new BaseConfiguration(
    Name:ConfigurationFixture.GetValidName(),
    Value: ConfigurationFixture.GetValidValue(),
    Description: ConfigurationFixture.GetValidDescription(),
    StartDate: ConfigurationFixture.GetValidStartDate(),
    ExpireDate: DateTime.UtcNow.AddSeconds(-10)), // ExpireDate is less than now
DomainErrorCode.Validation,
nameof(Configuration.ExpireDate)
        };
    }

    public IEnumerator<object[]> GetEnumerator() => (IEnumerator<object[]>)GetPersonFromDataGenerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}