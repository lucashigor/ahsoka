using Ahsoka.Application.Administrations.Configurations.Commands;
using Ahsoka.Domain.Common.ValuesObjects;
using Ahsoka.Domain.Entities.Admin.Configurations;
using Ahsoka.Domain.Entities.Admin.Configurations.Events;
using Ahsoka.TestsUtil;
using Ahsoka.Unit.Tests.Domain.Entities.Admin.Configurations.Helpers;
using FluentAssertions;
using System.Collections;
using Xunit;

namespace Ahsoka.Unit.Tests.Domain.Entities.Admin.Configurations;

[Collection(nameof(ConfigurationTestFixture))]
public class ConfigurationTests(ConfigurationTestFixture Fixture)
{
    [Fact(DisplayName = nameof(New_Configuration_ValidInput_ShouldNotHaveNotifications))]
    [Trait("Domain", "Configuration - Validation")]
    public void New_Configuration_ValidInput_ShouldNotHaveNotifications()
    {
        // Act
        var configuration = ConfigurationFixture.GetValidConfiguration(Guid.NewGuid());

        // Assert
        configuration.Events.Should().Contain(x => x.EventName == nameof(ConfigurationCreatedDomainEvent), "New configuration should raise Created Domain Event");
    }

    [Theory]
    [MemberData(nameof(TestInvalidDataGenerator.GetPersonFromDataGenerator), MemberType = typeof(TestInvalidDataGenerator))]
    public void ValidationErrorsOnNewConfiguration(BaseConfiguration inputConfig, DomainErrorCode error, string fieldName)
    {
        var (result, config) = Configuration.New(inputConfig.Name, inputConfig.Value, inputConfig.Description, inputConfig.StartDate, inputConfig.ExpireDate, Guid.NewGuid().ToString());

        result.IsFailure.Should().BeTrue();

        config.Should().BeNull();

        result!.Errors.Should().Contain(x => x.Error == error);
        result!.Errors.Should().Contain(x => x.FieldName.Equals(fieldName, StringComparison.InvariantCultureIgnoreCase));
    }
}
