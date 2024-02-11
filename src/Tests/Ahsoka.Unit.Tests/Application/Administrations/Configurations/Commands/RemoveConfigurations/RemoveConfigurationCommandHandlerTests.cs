using Ahsoka.Application.Administrations.Configurations.Commands.RemoveConfiguration;
using Ahsoka.Application.Common.Interfaces;
using Ahsoka.Application.Common.Models;
using Ahsoka.Application.Dto.Administrations.Configurations.ApplicationsErrors;
using Ahsoka.Domain.Entities.Admin.Configurations;
using Ahsoka.Domain.Entities.Admin.Configurations.Repository;
using Ahsoka.TestsUtil;
using Ahsoka.Unit.Tests.Domain.Entities.Admin.Configurations;
using FluentAssertions;
using NSubstitute;

namespace Ahsoka.Unit.Tests.Application.Administrations.Configurations.Commands.RemoveConfigurations;
[Collection(nameof(ConfigurationTestFixture))]
public class RemoveConfigurationCommandHandlerTests
{
    private readonly IConfigurationRepository _configurationRepository;
    private readonly Notifier _notifier;
    private readonly IUnitOfWork _unitOfWork;

    public RemoveConfigurationCommandHandlerTests(ConfigurationTestFixture fixture)
    {
        _configurationRepository = Substitute.For<IConfigurationRepository>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _notifier = new Notifier();
    }

    [Fact(DisplayName = nameof(HandleRemoveConfigurationCommandHandler_NotFoundAsync))]
    [Trait("Domain", "Configuration - RemoveConfigurationCommandHandler")]
    public async Task HandleRemoveConfigurationCommandHandler_NotFoundAsync()
    {
        var app = GetApp();

        var command = GetCommand(Guid.NewGuid());

        await app.Handle(command, CancellationToken.None);

        _notifier.Warnings.Should().NotBeEmpty();
        _notifier.Warnings.Should().Contain(x => x.Code == ConfigurationErrorCodes.NotFound);
        await _unitOfWork.Received(0).CommitAsync(Arg.Any<CancellationToken>());
        await _configurationRepository.Received(0).UpdateAsync(Arg.Any<Configuration>(), Arg.Any<CancellationToken>());
    }

    private RemoveConfigurationCommand GetCommand(ConfigurationId Id)
        => new(Id);

    private RemoveConfigurationCommandHandler GetApp()
        => new(_configurationRepository,
                _unitOfWork,
                _notifier);
}
