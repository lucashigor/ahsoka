using Ahsoka.Application.Common.Models;
using FluentValidation;

namespace Ahsoka.Application.Administrations.Configurations.Commands.RemoveConfiguration;

public class RemoveConfigurationCommandValidator : AbstractValidator<RemoveConfigurationCommand>
{
    public RemoveConfigurationCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(ValidationConstant.RequiredField);
    }
}