using Ahsoka.Application.Administrations.Configurations.Commands.ChangeConfiguration;
using Ahsoka.Application.Common.Models;
using FluentValidation;

namespace Ahsoka.Application.Administrations.Configurations.Commands.ModifyConfiguration;

public class ChangeConfigurationCommandValidator : AbstractValidator<ChangeConfigurationCommand>
{
    public ChangeConfigurationCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(ValidationConstant.RequiredField);

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage(ValidationConstant.RequiredField);

        RuleFor(x => x.Value)
            .NotEmpty()
            .WithMessage(ValidationConstant.RequiredField);
    }
}
