// Ignore Spelling: Validator

using Ahsoka.Application.Administrations.Configurations.Commands.RegisterConfiguration;
using Ahsoka.TestsUtil;
using Ahsoka.Unit.Tests.Domain.Entities.Admin.Configurations;
using FluentAssertions;
using FluentValidation.TestHelper;

namespace Ahsoka.Unit.Tests.Application.Administrations.Configurations.Commands.RegisterConfigurations;

[Collection(nameof(ConfigurationTestFixture))]
public class RegisterConfigurationCommandValidatorTests()
{
    private readonly RegisterConfigurationCommandValidator _validator = new();

    [Fact]
    public void Name_ShouldHaveError_WhenNameIsEmpty()
    {
        var command = GetCommand(name: "", value: ConfigurationFixture.GetValidValue());
        var result = _validator.TestValidate(command);
        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(config => config.Name);
    }

    [Fact]
    public void Value_ShouldHaveError_WhenValueIsEmpty()
    {
        var command = GetCommand(name: ConfigurationFixture.GetValidName(), value: "");
        var result = _validator.TestValidate(command);
        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(config => config.Value);
    }

    [Fact]
    public void ShouldNotHaveError_WhenNameAndValueAreNotEmpty()
    {
        var command = GetCommand(name: ConfigurationFixture.GetValidName(), value: ConfigurationFixture.GetValidValue());
        var result = _validator.TestValidate(command);
        result.IsValid.Should().BeTrue();
    }

    private static RegisterConfigurationCommand GetCommand(string? name, string? value)
        => new(ConfigurationTestFixture.GetBaseConfiguration(name, value));
}