using FluentValidation;
using invenpro.auth.request.Auth.Commands;

namespace invenpro.auth.requestvalidator.Auth;

public sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.Email)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Email es requerido")
            .EmailAddress().WithMessage("Email inválido")
            .MaximumLength(255);

        RuleFor(x => x.Password)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Password es requerido")
            .MinimumLength(6).WithMessage("Password debe tener al menos 6 caracteres")
            .MaximumLength(100);
    }
}