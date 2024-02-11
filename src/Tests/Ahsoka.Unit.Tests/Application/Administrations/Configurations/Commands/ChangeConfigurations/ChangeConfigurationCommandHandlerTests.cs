using Ahsoka.Application.Administrations.Configurations.Commands.ChangeConfiguration;
using Ahsoka.Application.Administrations.Configurations.Services;
using Ahsoka.Application.Common.Interfaces;
using Ahsoka.Application.Common.Models;
using Ahsoka.Domain.Entities.Admin.Configurations;
using Ahsoka.Domain.Entities.Admin.Configurations.Repository;
using Ahsoka.TestsUtil;
using Ahsoka.Unit.Tests.Domain.Entities.Admin.Configurations;
using FluentAssertions;
using NSubstitute;

namespace Ahsoka.Unit.Tests.Application.Administrations.Configurations.Commands.ChangeConfigurations;

[Collection(nameof(ConfigurationTestFixture))]
public class ChangeConfigurationCommandHandlerTests
{
    private readonly IConfigurationRepository _configurationRepository;
    private readonly Notifier _notifier;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IConfigurationServices _configurationServices;
    private readonly ConfigurationTestFixture _fixture;

    public ChangeConfigurationCommandHandlerTests(ConfigurationTestFixture fixture)
    {
        _configurationRepository = Substitute.For<IConfigurationRepository>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _configurationServices = Substitute.For<IConfigurationServices>();
        _notifier = new Notifier();
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(HandleChangeConfigurationCommandHandlerAsync))]
    [Trait("Domain", "Configuration - ChangeConfigurationCommandHandler")]
    public async Task HandleChangeConfigurationCommandHandlerAsync()
    {
        var app = GetApp();

        var config = ConfigurationFixture.GetValidConfiguration(ConfigurationStatus.Awaiting);

        _configurationRepository.GetByIdAsync(Arg.Is<ConfigurationId>(id => id == config.Id),
            Arg.Any<CancellationToken>()).Returns(config);

        var newName = ConfigurationFixture.GetValidName();

        var command = GetCommand(config.Id, newName);

        await app.Handle(command, CancellationToken.None);

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

        var config = ConfigurationFixture.GetValidConfiguration(ConfigurationStatus.Awaiting);

        var newName = ConfigurationFixture.GetValidName();

        var command = GetCommand(config.Id, newName);

        await app.Handle(command, CancellationToken.None);

        _notifier.Errors.Should().NotBeEmpty();
        await _configurationServices.Received(0).Handle(Arg.Any<Configuration>(), Arg.Any<CancellationToken>());
        await _unitOfWork.Received(0).CommitAsync(Arg.Any<CancellationToken>());
        await _configurationRepository.Received(0).UpdateAsync(Arg.Any<Configuration>(), Arg.Any<CancellationToken>());
    }

    private ChangeConfigurationCommand GetCommand(ConfigurationId Id, string? name)
        => new(Id, _fixture.GetDtoBaseConfiguration(name));

    private ChangeConfigurationCommandHandler GetApp()
        => new(_configurationRepository,
                _unitOfWork,
                _notifier,
                _configurationServices);
}
