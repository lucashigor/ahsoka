namespace Ahsoka.Integrations.Tests.Administrations.Configurations.Commands;

public class ConfigurationTestFixture : IntegrationsTestsFixture
{
    [CollectionDefinition(nameof(ConfigurationTestFixture))]
    public class ConfigurationTestFixtureCollection : ICollectionFixture<ConfigurationTestFixture>
    {
    }
}
