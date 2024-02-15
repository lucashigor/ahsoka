namespace Ahsoka.Unit.Tests.Application.Administrations.Configurations.Commands.RegisterConfigurations;

using Ahsoka.Application.Administrations.Configurations.Commands.RegisterConfiguration;
using Ahsoka.Application.Administrations.Configurations.Services;
using Ahsoka.Application.Common.Interfaces;
using Ahsoka.Application.Common.Models;
using Ahsoka.Domain.Entities.Admin.Configurations;
using Ahsoka.Domain.Entities.Admin.Configurations.Repository;
using Ahsoka.TestsUtil;
using Ahsoka.Unit.Tests.Domain.Entities.Admin.Configurations;
using FluentAssertions;
using NSubstitute;
using System.Threading.Tasks;
using Xunit;

[Collection(nameof(ConfigurationTestFixture))]
public class RegisterConfigurationCommandHandlerTests
{
    private readonly ICommandsConfigurationRepository _configurationRepository;
    private readonly Notifier _notifier;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IConfigurationServices _configurationServices;
    private readonly ConfigurationTestFixture _fixture;

    public RegisterConfigurationCommandHandlerTests(ConfigurationTestFixture fixture)
    {
        _configurationRepository = Substitute.For<ICommandsConfigurationRepository>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _configurationServices = Substitute.For<IConfigurationServices>();
        _notifier = new Notifier();
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(HandleRegisterConfigurationAsync))]
    [Trait("Domain", "Configuration - RegisterConfigurationCommandHandler")]
    public async Task HandleRegisterConfigurationAsync()
    {
        var app = GetApp();

        var command = GetCommand(null);

        await app.Handle(command, CancellationToken.None);

        _notifier.Errors.Should().BeEmpty();
        await _configurationServices.Received(1).Handle(Arg.Any<Configuration>(), Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).CommitAsync(Arg.Any<CancellationToken>());
        await _configurationRepository.Received(1).InsertAsync(Arg.Any<Configuration>(), Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = nameof(HandleRegisterConfigurationAsync_WithError))]
    [Trait("Domain", "Configuration - RegisterConfigurationCommandHandler")]
    public async Task HandleRegisterConfigurationAsync_WithError()
    {
        var app = GetApp();

        var command = GetCommand("");

        await app.Handle(command, CancellationToken.None);

        _notifier.Errors.Should().NotBeEmpty();
        await _configurationServices.Received(0).Handle(Arg.Any<Configuration>(), Arg.Any<CancellationToken>());
        await _unitOfWork.Received(0).CommitAsync(Arg.Any<CancellationToken>());
        await _configurationRepository.Received(0).InsertAsync(Arg.Any<Configuration>(), Arg.Any<CancellationToken>());
    }

    private RegisterConfigurationCommand GetCommand(string? name)
        => new(_fixture.GetDtoBaseConfiguration(name));

    private RegisterConfigurationCommandHandler GetApp()
        => new(_configurationRepository,
                _unitOfWork,
                _notifier,
                _configurationServices,
                GeneralFixture.GetUserService());
}
