using Ahsoka.Application.Common.Models;
using FluentValidation;

namespace Ahsoka.Application.Administrations.Configurations.Commands.RegisterConfiguration;

public class RegisterConfigurationCommandValidator : AbstractValidator<RegisterConfigurationCommand>
{
    public RegisterConfigurationCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage(ValidationConstant.RequiredField);

        RuleFor(x => x.Value)
            .NotEmpty()
            .WithMessage(ValidationConstant.RequiredField);
    }
}
