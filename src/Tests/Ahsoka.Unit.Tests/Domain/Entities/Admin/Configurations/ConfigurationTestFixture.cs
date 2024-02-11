using Ahsoka.Application.Dto.Administrations.Configurations.Requests;
using Ahsoka.Domain.Entities.Admin.Configurations;
using Ahsoka.TestsUtil;

namespace Ahsoka.Unit.Tests.Domain.Entities.Admin.Configurations;

public class ConfigurationTestFixture
{
    public BaseConfiguration GetDtoBaseConfiguration(string? name)
        => new (
            Name: name ?? ConfigurationFixture.GetValidName(),
            Value: ConfigurationFixture.GetValidValue(),
            Description: ConfigurationFixture.GetValidDescription(),
            StartDate: ConfigurationFixture.GetValidStartDate(ConfigurationStatus.Awaiting),
            ExpireDate: ConfigurationFixture.GetValidExpireDate(ConfigurationStatus.Awaiting)
                );

    [CollectionDefinition(nameof(ConfigurationTestFixture))]
    public class ConfigurationTestFixtureCollection : ICollectionFixture<ConfigurationTestFixture>
    {
    }
}
