using FluentValidation;
using invenpro.auth.common.Validations;

namespace invenpro.auth.requestvalidator.Custom.RuleValidators;

public static partial class ScripTextValidator
{
    public static IRuleBuilderOptions<T, string> NotScriptSyntax<T>(this IRuleBuilder<T, string> ruleBuilder)
        => ruleBuilder.Must(ContaintsScriptSyntax);

    private static bool ContaintsScriptSyntax(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return false;
        return RegexValidator.NoScriptRegex().IsMatch(value);
    }
}