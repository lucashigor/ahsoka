using Ahsoka.Application.Administrations.Configurations.Commands.ChangeConfiguration;
using Ahsoka.Application.Administrations.Configurations.Services;
using Ahsoka.Application.Common.Interfaces;
using Ahsoka.Application.Common.Models;
using Ahsoka.Application.Dto.Administrations.Configurations.ApplicationsErrors;
using Ahsoka.Domain.Entities.Admin.Configurations;
using Ahsoka.Domain.Entities.Admin.Configurations.Repository;
using Ahsoka.TestsUtil;
using Ahsoka.Unit.Tests.Domain.Entities.Admin.Configurations;
using FluentAssertions;
using NSubstitute;

namespace Ahsoka.Unit.Tests.Application.Administrations.Configurations.Commands.ChangeConfigurations;

[Collection(nameof(ConfigurationTestFixture))]
public class ChangeConfigurationCommandHandlerTests(ConfigurationTestFixture fixture)
{
    private readonly ICommandsConfigurationRepository _configurationRepository = Substitute.For<ICommandsConfigurationRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IConfigurationServices _configurationServices = Substitute.For<IConfigurationServices>();
    private readonly ConfigurationTestFixture _fixture = fixture;

    [Fact(DisplayName = nameof(HandleChangeConfigurationCommandHandlerAsync))]
    [Trait("Domain", "Configuration - ChangeConfigurationCommandHandler")]
    public async Task HandleChangeConfigurationCommandHandlerAsync()
    {
        var app = GetApp();

        var config = ConfigurationFixture.GetValidConfiguration(ConfigurationState.Awaiting);

        _configurationRepository.GetByIdAsync(Arg.Is<ConfigurationId>(id => id == config.Id),
            Arg.Any<CancellationToken>()).Returns(config);

        var newName = ConfigurationFixture.GetValidName();

        var command = GetCommand(config.Id, newName);

        var _notifier = await app.Handle(command, CancellationToken.None);

        _notifier.Errors.Should().BeEmpty();
        await _configurationServices.Received(1).Handle(Arg.Any<Configuration>(), Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).CommitAsync(Arg.Any<CancellationToken>());
        await _configurationRepository.Received(1).UpdateAsync(Arg.Is<Configuration>(config => config.Name == newName), Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = nameof(HandleChangeConfigurationCommandHandler_NotFoundAsync))]
    [Trait("Domain", "Configuration - ChangeConfigurationCommandHandler")]
    public async Task HandleChangeConfigurationCommandHandler_NotFoundAsync()
    {
        var app = GetApp();

        var config = ConfigurationFixture.GetValidConfiguration(ConfigurationState.Awaiting);

        var newName = ConfigurationFixture.GetValidName();

        var command = GetCommand(config.Id, newName);

        var _notifier = await app.Handle(command, CancellationToken.None);

        _notifier.Errors.Should().NotBeEmpty();
        _notifier.Errors.Should().Contain(x => x.Code == ConfigurationErrorCodes.NotFound);
        await _configurationServices.Received(0).Handle(Arg.Any<Configuration>(), Arg.Any<CancellationToken>());
        await _unitOfWork.Received(0).CommitAsync(Arg.Any<CancellationToken>());
        await _configurationRepository.Received(0).UpdateAsync(Arg.Any<Configuration>(), Arg.Any<CancellationToken>());
    }


    [Fact(DisplayName = nameof(HandleChangeConfigurationCommandHandler_NotFoundAsync))]
    [Trait("Domain", "Configuration - ChangeConfigurationCommandHandler")]
    public async Task HandleChangeConfigurationCommandHandler_NameInvalidAsync()
    {
        var app = GetApp();

        var config = ConfigurationFixture.GetValidConfiguration(ConfigurationState.Awaiting);

        _configurationRepository.GetByIdAsync(Arg.Is<ConfigurationId>(id => id == config.Id),
            Arg.Any<CancellationToken>()).Returns(config);

        var command = GetCommand(config.Id, "");

        var _notifier = await app.Handle(command, CancellationToken.None);

        _notifier.Errors.Should().NotBeEmpty();
        _notifier.Errors.Should().Contain(x => x.Code == ConfigurationErrorCodes.ConfigurationsValidation);
        await _configurationServices.Received(0).Handle(Arg.Any<Configuration>(), Arg.Any<CancellationToken>());
        await _unitOfWork.Received(0).CommitAsync(Arg.Any<CancellationToken>());
        await _configurationRepository.Received(0).UpdateAsync(Arg.Any<Configuration>(), Arg.Any<CancellationToken>());
    }

    private static ChangeConfigurationCommand GetCommand(ConfigurationId Id, string? name)
        => new(Id, ConfigurationTestFixture.GetBaseConfiguration(name));

    private ChangeConfigurationCommandHandler GetApp()
        => new(_configurationRepository,
                _unitOfWork,
                _configurationServices);
}
