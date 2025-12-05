using FluentValidation;
using invenpro.auth.request.Auth.Commands;

namespace invenpro.auth.requestvalidator.Auth;

public sealed class LogoutCommandValidator : AbstractValidator<LogoutCommand>
{
    public LogoutCommandValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.UserId)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("UserId es requerido")
            .MaximumLength(50);
    }
}