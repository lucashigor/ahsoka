using Ahsoka.Application.Administrations.Configurations.Commands.ChangeConfiguration;
using Ahsoka.Application.Administrations.Configurations.Commands.ModifyConfiguration;
using Ahsoka.Domain.Entities.Admin.Configurations;
using Ahsoka.TestsUtil;
using Ahsoka.Unit.Tests.Domain.Entities.Admin.Configurations;
using FluentAssertions;
using FluentValidation.TestHelper;

namespace Ahsoka.Unit.Tests.Application.Administrations.Configurations.Commands.ChangeConfigurations;

[Collection(nameof(ConfigurationTestFixture))]
public class ChangeConfigurationCommandValidatorTests
{
    private readonly ChangeConfigurationCommandValidator _validator;
    private readonly ConfigurationTestFixture _fixture;

    public ChangeConfigurationCommandValidatorTests(ConfigurationTestFixture fixture)
    {
        _validator = new ChangeConfigurationCommandValidator();
        _fixture = fixture;
    }

    [Fact]
    public void Name_ShouldHaveError_WhenNameIsEmpty()
    {
        var command = GetCommand(ConfigurationId.New(), name: "", value: ConfigurationFixture.GetValidValue());
        var result = _validator.TestValidate(command);
        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(config => config.Name);
    }

    [Fact]
    public void Value_ShouldHaveError_WhenValueIsEmpty()
    {
        var command = GetCommand(ConfigurationId.New(), name: ConfigurationFixture.GetValidName(), value: "");
        var result = _validator.TestValidate(command);
        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(config => config.Value);
    }

    [Fact]
    public void ShouldNotHaveError_WhenNameAndValueAreNotEmpty()
    {
        var command = GetCommand(ConfigurationId.New(), name: ConfigurationFixture.GetValidName(), value: ConfigurationFixture.GetValidValue());
        var result = _validator.TestValidate(command);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void ShouldNotHaveError_WhenIdEmpty()
    {
        var command = GetCommand(Guid.Empty, name: ConfigurationFixture.GetValidName(), value: ConfigurationFixture.GetValidValue());
        var result = _validator.TestValidate(command);
        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(config => config.Id);
    }

    private ChangeConfigurationCommand GetCommand(ConfigurationId Id, string? name, string? value)
        => new(Id, _fixture.GetDtoBaseConfiguration(name, value));
}
