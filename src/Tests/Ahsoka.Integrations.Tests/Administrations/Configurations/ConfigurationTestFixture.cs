namespace Ahsoka.Integrations.Tests.Administrations.Configurations;

public class ConfigurationTestFixture : IntegrationsTestsFixture
{
    [CollectionDefinition(nameof(ConfigurationTestFixture))]
    public class ConfigurationTestFixtureCollection : ICollectionFixture<ConfigurationTestFixture>
    {
    }
}
