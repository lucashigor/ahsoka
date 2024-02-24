using Ahsoka.Application.Administrations.Configurations.Commands;

namespace Ahsoka.Integrations.Tests.Administrations.Configurations.Queries;

[Collection(nameof(ConfigurationTestFixture))]
public class QueriesConfigurationRepositoryTests(ConfigurationTestFixture fixture)
{
    [Fact(DisplayName = nameof(GetConfigurationByIdAsync))]
    [Trait("Integration", "Configuration - QueriesConfigurationRepository")]
    public async Task GetConfigurationByIdAsync()
    {
        var (dbOptions, repository) = GetGenericRepositoryTest();

        using var context = new PrincipalContext(dbOptions);
        var config = ConfigurationFixture.GetValidConfiguration();

        context.Configuration.Add(config);
        context.SaveChanges();

        var result = await repository.GetByIdAsync(config.Id, CancellationToken.None);

        result.Should().NotBeNull();
    }

    [Fact(DisplayName = nameof(GetConfigurationListAsync))]
    [Trait("Integration", "Configuration - QueriesConfigurationRepository")]
    public async Task GetConfigurationListAsync()
    {
        var (dbOptions, repository) = GetGenericRepositoryTest();

        using var context = new PrincipalContext(dbOptions);

        var name = ConfigurationFixture.GetValidName();

        await GetListConfigurations(name);

        var result = await repository.GetAllByNameAsync(name, [ConfigurationState.Awaiting], CancellationToken.None);

        result.Should().HaveCount(1);
    }

    internal async Task GetListConfigurations(string name)
    {
        var dbOptions = IntegrationsTestsFixture.CreateDatabase();
        using var context = new PrincipalContext(dbOptions);

        var list = new List<Configuration>();

        var configAwaiting = ConfigurationFixture.LoadConfiguration(
            new BaseConfiguration(
            Name: name,
            Value: ConfigurationFixture.GetValidValue(),
            Description: ConfigurationFixture.GetValidDescription(),
            StartDate: ConfigurationFixture.GetValidStartDate(ConfigurationState.Awaiting),
            ExpireDate: ConfigurationFixture.GetValidExpireDate(ConfigurationState.Awaiting)), Guid.NewGuid());

        var configActive = ConfigurationFixture.LoadConfiguration(
            new BaseConfiguration(
            Name: name,
            Value: ConfigurationFixture.GetValidValue(),
            Description: ConfigurationFixture.GetValidDescription(),
            StartDate: ConfigurationFixture.GetValidStartDate(ConfigurationState.Active),
            ExpireDate: ConfigurationFixture.GetValidExpireDate(ConfigurationState.Active)), Guid.NewGuid());

        var configExpired = ConfigurationFixture.LoadConfiguration(
            new BaseConfiguration(
            Name: name,
            Value: ConfigurationFixture.GetValidValue(),
            Description: ConfigurationFixture.GetValidDescription(),
            StartDate: ConfigurationFixture.GetValidStartDate(ConfigurationState.Expired),
            ExpireDate: ConfigurationFixture.GetValidExpireDate(ConfigurationState.Expired)), Guid.NewGuid());

        context.Configuration.AddRange([configAwaiting, configActive, configExpired]);

        await context.SaveChangesAsync();
    }

    private (DbContextOptions<PrincipalContext>, QueriesConfigurationRepository)
        GetGenericRepositoryTest()
    {
        var dbOptions = IntegrationsTestsFixture.CreateDatabase();

        var context = new PrincipalContext(dbOptions);

        var repository = new QueriesConfigurationRepository(context);
        return (dbOptions, repository);
    }
}
