using Ahsoka.Application.Administrations.Configurations.Commands;
using Ahsoka.Application.Administrations.Configurations.Services;
using Ahsoka.Application.Common.Models;
using Ahsoka.Application.Dto.Administrations.Configurations.ApplicationsErrors;
using Ahsoka.Domain.Entities.Admin.Configurations;
using Ahsoka.Domain.Entities.Admin.Configurations.Repository;
using Ahsoka.TestsUtil;
using Ahsoka.Unit.Tests.Domain.Entities.Admin.Configurations;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ahsoka.Unit.Tests.Application.Administrations.Configurations.Services;

[Collection(nameof(ConfigurationTestFixture))]
public class ConfigurationServicesTests
{
    private readonly IConfigurationRepository _configurationRepository;
    private readonly Notifier _notifier;

    public ConfigurationServicesTests(ConfigurationTestFixture fixture)
    {
        _configurationRepository = Substitute.For<IConfigurationRepository>();
        _notifier = new Notifier();
    }

    [Fact(DisplayName = nameof(HandleDatesWithEmptyDatabaseAsync))]
    [Trait("Domain", "Configuration - DateValidationServices")]
    public async Task HandleDatesWithEmptyDatabaseAsync()
    {
        var validData = ConfigurationFixture.GetValidConfiguration();

        var app = new ConfigurationServices(_configurationRepository, _notifier);

        await app.Handle(validData, CancellationToken.None);

        _notifier.Errors.Should().BeEmpty();
    }

    [Fact(DisplayName = nameof(HandleDatesWithSameNameClosedBeforeDatabaseAsync))]
    [Trait("Domain", "Configuration - DateValidationServices")]
    public async Task HandleDatesWithSameNameClosedBeforeDatabaseAsync()
    {
        var validData = ConfigurationFixture.GetValidConfiguration();

        _configurationRepository.GetAllByNameAsync(Arg.Is<string>(name => name == validData.Name),
            Arg.Is<ConfigurationStatus[]>(statuses => statuses.Contains(ConfigurationStatus.Active) || statuses.Contains(ConfigurationStatus.Awaiting)),
            Arg.Any<CancellationToken>()).Returns(new List<Configuration>());

        var app = new ConfigurationServices(_configurationRepository, _notifier);

        await app.Handle(validData, CancellationToken.None);

        _notifier.Errors.Should().BeEmpty();
    }

    [Fact(DisplayName = nameof(HandleDatesWithSameNameStartsAfterDatabaseAsync))]
    [Trait("Domain", "Configuration - DateValidationServices")]
    public async Task HandleDatesWithSameNameStartsAfterDatabaseAsync()
    {
        var validData = ConfigurationFixture.GetValidConfiguration();

        var beforeConfig = ConfigurationFixture.LoadConfiguration(new BaseConfiguration(
            Name: validData.Name,
            Value: validData.Value,
            Description: validData.Description,
            StartDate: validData.ExpireDate!.Value.AddDays(10),
            ExpireDate: validData.ExpireDate.Value.AddDays(20)
        ));

        _configurationRepository.GetAllByNameAsync(Arg.Is<string>(name => name == validData.Name),
            Arg.Is<ConfigurationStatus[]>(statuses => statuses.Contains(ConfigurationStatus.Active) || statuses.Contains(ConfigurationStatus.Awaiting)),
            Arg.Any<CancellationToken>()).Returns(new List<Configuration>() { beforeConfig });

        var app = new ConfigurationServices(_configurationRepository, _notifier);

        await app.Handle(validData, CancellationToken.None);

        _notifier.Errors.Should().BeEmpty();
    }

    [Fact(DisplayName = nameof(HandleDatesWithSameNameOpeningDuringDatabaseAsync))]
    [Trait("Domain", "Configuration - DateValidationServices")]
    public async Task HandleDatesWithSameNameOpeningDuringDatabaseAsync()
    {
        var validData = ConfigurationFixture.GetValidConfiguration();
                
        var beforeConfig = ConfigurationFixture.LoadConfiguration(new BaseConfiguration(
            Name: validData.Name,
            Value: validData.Value,
            Description: validData.Description,
            StartDate: validData.StartDate.AddDays(-3),
            ExpireDate: validData.StartDate.AddDays(1)
        ));

        _configurationRepository.GetAllByNameAsync(Arg.Is<string>(name => name == validData.Name),
            Arg.Is<ConfigurationStatus[]>(statuses => statuses.Contains(ConfigurationStatus.Active) || statuses.Contains(ConfigurationStatus.Awaiting)),
            Arg.Any<CancellationToken>()).Returns(new List<Configuration>() { beforeConfig });


        var app = new ConfigurationServices(_configurationRepository, _notifier);
        await app.Handle(validData, CancellationToken.None);

        _notifier.Errors.Should().HaveCount(1);
        _notifier.Errors[0].Code.Should().Be(Ahsoka.Application.Dto.Common.ApplicationsErrors.Errors.ThereWillCurrentConfigurationStartDate().Code);
        _notifier.Errors[0].Message.Should().Be(Ahsoka.Application.Dto.Common.ApplicationsErrors.Errors.ThereWillCurrentConfigurationStartDate().Message);
    }

    [Fact(DisplayName = nameof(HandleDatesWithSameNameClosingDuringDatabaseAsync))]
    [Trait("Domain", "Configuration - DateValidationServices")]
    public async Task HandleDatesWithSameNameClosingDuringDatabaseAsync()
    {
        var validData = ConfigurationFixture.GetValidConfiguration();

        var beforeConfig = ConfigurationFixture.LoadConfiguration(new BaseConfiguration(
            Name: validData.Name,
            Value: validData.Value,
            Description: validData.Description,
            StartDate: validData!.ExpireDate!.Value.AddDays(-1),
            ExpireDate: validData!.ExpireDate!.Value.AddDays(1)
        ));

        _configurationRepository.GetAllByNameAsync(Arg.Is<string>(name => name == validData.Name),
            Arg.Is<ConfigurationStatus[]>(statuses => statuses.Contains(ConfigurationStatus.Active) || statuses.Contains(ConfigurationStatus.Awaiting)),
            Arg.Any<CancellationToken>()).Returns(new List<Configuration>() { beforeConfig });


        var app = new ConfigurationServices(_configurationRepository, _notifier);
        await app.Handle(validData, CancellationToken.None);

        _notifier.Errors.Should().HaveCount(1);
        _notifier.Errors[0].Code.Should().Be(Ahsoka.Application.Dto.Common.ApplicationsErrors.Errors.ThereWillCurrentConfigurationEndDate().Code);
        _notifier.Errors[0].Message.Should().Be(Ahsoka.Application.Dto.Common.ApplicationsErrors.Errors.ThereWillCurrentConfigurationEndDate().Message);
    }

    [Fact(DisplayName = nameof(HandleDatesWithSameNameDuringDatabaseAsync))]
    [Trait("Domain", "Configuration - DateValidationServices")]
    public async Task HandleDatesWithSameNameDuringDatabaseAsync()
    {
        var validData = ConfigurationFixture.GetValidConfiguration();

        var beforeConfig = ConfigurationFixture.LoadConfiguration(new BaseConfiguration(
            Name: validData.Name,
            Value: validData.Value,
            Description: validData.Description,
            StartDate: validData.StartDate.AddDays(-3),
            ExpireDate: validData!.ExpireDate!.Value.AddDays(3)
        ));

        _configurationRepository.GetAllByNameAsync(Arg.Is<string>(name => name == validData.Name),
            Arg.Is<ConfigurationStatus[]>(statuses => statuses.Contains(ConfigurationStatus.Active) || statuses.Contains(ConfigurationStatus.Awaiting)),
            Arg.Any<CancellationToken>()).Returns(new List<Configuration>() { beforeConfig });


        var app = new ConfigurationServices(_configurationRepository, _notifier);
        await app.Handle(validData, CancellationToken.None);

        _notifier.Errors.Should().HaveCount(2);
        _notifier.Errors[0].Code.Should().Be(Ahsoka.Application.Dto.Common.ApplicationsErrors.Errors.ThereWillCurrentConfigurationStartDate().Code);
        _notifier.Errors[0].Message.Should().Be(Ahsoka.Application.Dto.Common.ApplicationsErrors.Errors.ThereWillCurrentConfigurationStartDate().Message);

        _notifier.Errors[1].Code.Should().Be(Ahsoka.Application.Dto.Common.ApplicationsErrors.Errors.ThereWillCurrentConfigurationEndDate().Code);
        _notifier.Errors[1].Message.Should().Be(Ahsoka.Application.Dto.Common.ApplicationsErrors.Errors.ThereWillCurrentConfigurationEndDate().Message);
    }
}