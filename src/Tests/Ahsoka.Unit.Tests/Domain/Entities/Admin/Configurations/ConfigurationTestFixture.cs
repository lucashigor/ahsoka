using Xunit;

namespace Ahsoka.Unit.Tests.Domain.Entities.Admin.Configurations;

public class ConfigurationTestFixture
{
    [CollectionDefinition(nameof(ConfigurationTestFixture))]
    public class ConfigurationTestFixtureCollection : ICollectionFixture<ConfigurationTestFixture>
    {
    }
}
