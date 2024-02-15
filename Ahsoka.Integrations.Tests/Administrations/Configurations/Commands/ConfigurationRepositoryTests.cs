namespace Ahsoka.Integrations.Tests.Administrations.Configurations.Commands;

[Collection(nameof(ConfigurationTestFixture))]
public class ConfigurationRepositoryTests(ConfigurationTestFixture fixture)
{
    private readonly ConfigurationTestFixture _fixture = fixture;

    [Fact(DisplayName = nameof(InsertNewConfigurationAsync))]
    [Trait("Integration", "Configuration - ConfigurationRepository")]
    public async void InsertNewConfigurationAsync()
    {
        var (_, repository) = GetGenericRepositoryTest();

        var result = await repository.CreateEntityAsync(() => 
            ConfigurationFixture.GetValidConfiguration());

        result.Should().BeTrue();
    }

    [Fact(DisplayName = nameof(GetByIdConfigurationAsync))]
    [Trait("Integration", "Configuration - ConfigurationRepository")]
    public async void GetByIdConfigurationAsync()
    {
        var (config, repository) = GetGenericRepositoryTest();

        var result = await repository.GetEntityByIdAsync(() => 
            ConfigurationFixture.GetValidConfigurationAtDatabase(
                config,
                ConfigurationState.Awaiting,
                Guid.NewGuid()));

        result.Should().BeTrue();
    }

    [Fact(DisplayName = nameof(DeleteConfigurationAsync))]
    [Trait("Integration", "Configuration - ConfigurationRepository")]
    public async void DeleteConfigurationAsync()
    {
        var (config, repository) = GetGenericRepositoryTest();

        var result = await repository.DeleteEntityAsync(() => 
            ConfigurationFixture.GetValidConfigurationAtDatabase(
                config,
                ConfigurationState.Awaiting,
                Guid.NewGuid()));

        result.Should().BeTrue();
    }

    [Fact(DisplayName = nameof(UpdateConfigurationAsync))]
    [Trait("Integration", "Configuration - ConfigurationRepository")]
    public async void UpdateConfigurationAsync()
    {
        //Arrange
        var dbOptions = _fixture.CreateDatabase();

        var item = ConfigurationFixture.GetValidConfigurationAtDatabase(dbOptions,
            ConfigurationState.Awaiting,
            Guid.NewGuid());

        var context = new PrincipalContext(dbOptions);

        var app = new CommandsConfigurationRepository(context);
        var unitWork = new UnitOfWork(context);

        var oldDescription = item.Description;
        var newDescription = ConfigurationFixture.GetValidDescription();

        //Act
        item.Update(
            item.Name,
            item.Value,
            newDescription,
            item.StartDate,
            item.ExpireDate);

        await app.UpdateAsync(item, CancellationToken.None);

        //Assert
        using (var context2 = new PrincipalContext(dbOptions))
        {
            var database1 = context2.Configuration.Find(item.Id);
            database1?.Description.Should().Be(oldDescription,
                "should not affect before unit of work commit");
        }

        await unitWork.CommitAsync(CancellationToken.None);

        using (var context2 = new PrincipalContext(dbOptions))
        {
            var database1 = context2.Configuration.Find(item.Id);
            database1?.Description.Should().Be(newDescription);
        }
    }

    private (DbContextOptions<PrincipalContext>, RepositoryTestGeneric<Configuration, ConfigurationId, CommandsConfigurationRepository>)
        GetGenericRepositoryTest()
    {
        var dbOptions = _fixture.CreateDatabase();

        var repository = new RepositoryTestGeneric<Configuration, ConfigurationId, CommandsConfigurationRepository>(dbOptions);
        return (dbOptions, repository);
    }
}
