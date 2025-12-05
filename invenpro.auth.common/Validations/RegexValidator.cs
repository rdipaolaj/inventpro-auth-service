using invenpro.auth.common.Constants;
using System.Text.RegularExpressions;

namespace invenpro.auth.common.Validations;

public static partial class RegexValidator
{
    public static bool IsValidRegex(string pattern, string value)
    {
        Regex regex = new(pattern);
        return regex.IsMatch(value);
    }

    [GeneratedRegex(RegexPattern.NoScript)]
    public static partial Regex NoScriptRegex();

    [GeneratedRegex(RegexPattern.Ipv4)]
    public static partial Regex NoIpv4();
}