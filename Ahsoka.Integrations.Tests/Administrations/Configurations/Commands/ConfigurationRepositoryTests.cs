using Ahsoka.Domain.Entities.Admin.Configurations;
using Ahsoka.Infrastructure.Repositories.Common;
using Ahsoka.Infrastructure.Repositories.Configurations;
using Ahsoka.Infrastructure.Repositories.Context;
using Ahsoka.TestsUtil;
using FluentAssertions;

namespace Ahsoka.Integrations.Tests.Administrations.Configurations.Commands;

[Collection(nameof(ConfigurationTestFixture))]
public class ConfigurationRepositoryTests
{
    private readonly ConfigurationTestFixture _fixture;

    public ConfigurationRepositoryTests(ConfigurationTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(InsertNewConfigurationAsync))]
    [Trait("Integration", "Configuration - ConfigurationRepository")]
    public async void InsertNewConfigurationAsync()
    {
        //Arrange
        var dbOptions = _fixture.CreateDatabase();

        using PrincipalContext context = new(dbOptions);
        var app = new ConfigurationRepository(context);
        var item = ConfigurationFixture.GetValidConfiguration();

        //Act
        await app.InsertAsync(item, CancellationToken.None);

        //Assert
        using var context2 = new PrincipalContext(dbOptions);

        var database = context2.Configuration.Find(item.Id);

        database.Should().BeNull("should not affect before unit of work commit");

        var unit = new UnitOfWork(context);

        await unit.CommitAsync(CancellationToken.None);

        database = context2.Configuration.Find(item.Id);

        database.Should().NotBeNull();
    }

    [Fact(DisplayName = nameof(UpdateConfigurationAsync))]
    [Trait("Integration", "Configuration - ConfigurationRepository")]
    public async void UpdateConfigurationAsync()
    {
        //Arrange
        var dbOptions = _fixture.CreateDatabase();

        var item = ConfigurationFixture.GetValidConfigurationAtDatabase(dbOptions, 
            ConfigurationStatus.Awaiting, 
            Guid.NewGuid());

        var context = new PrincipalContext(dbOptions);

        var app = new ConfigurationRepository(context);
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

    [Fact(DisplayName = nameof(DeleteConfigurationAsync))]
    [Trait("Integration", "Configuration - ConfigurationRepository")]
    public async void DeleteConfigurationAsync()
    {
        //Arrange
        var dbOptions = _fixture.CreateDatabase();

        var item = ConfigurationFixture.GetValidConfigurationAtDatabase(dbOptions,
            ConfigurationStatus.Awaiting,
            Guid.NewGuid());

        using var context = new PrincipalContext(dbOptions);
        var app = new ConfigurationRepository(context);
        var unitWork = new UnitOfWork(context);

        //Act
        await app.DeleteAsync(item.Id, CancellationToken.None);

        //Assert
        var database = context.Configuration.Find(item.Id);

        database.Should().NotBeNull("should not affect before unit of work commit");

        await unitWork.CommitAsync(CancellationToken.None);

        database = context.Configuration.Find(item.Id);

        database.Should().BeNull();
    }

    [Fact(DisplayName = nameof(GetByIdConfigurationAsync))]
    [Trait("Integration", "Configuration - ConfigurationRepository")]
    public async void GetByIdConfigurationAsync()
    {
        //Arrange
        var dbOptions = _fixture.CreateDatabase();

        var item = ConfigurationFixture.GetValidConfigurationAtDatabase(dbOptions,
            ConfigurationStatus.Awaiting,
            Guid.NewGuid());

        using var context = new PrincipalContext(dbOptions);
        var app = new ConfigurationRepository(context);

        //Act
        var database = await app.GetByIdAsync(item.Id, CancellationToken.None);

        //Assert
        database.Should().NotBeNull();
    }
}
