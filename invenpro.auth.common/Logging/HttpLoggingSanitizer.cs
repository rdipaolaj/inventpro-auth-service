using Microsoft.Extensions.Primitives;
using System.Text.RegularExpressions;

namespace invenpro.auth.common.Logging;

public static class HttpLoggingSanitizer
{
    public static IDictionary<string, string> RedactHeaders(IEnumerable<KeyValuePair<string, StringValues>> headers, string[]? sensitive)
    {
        Dictionary<string, string> clean = new Dictionary<string, string>();
        foreach (KeyValuePair<string, StringValues> kv in headers)
        {
            string key = kv.Key;
            string val = kv.Value.ToString();
            bool isSensitive = sensitive is not null &&
                               Array.Exists(sensitive, s => string.Equals(s, key, StringComparison.OrdinalIgnoreCase));
            clean[key] = isSensitive ? Mask(val) : val;
        }
        return clean;
    }

    public static string RedactJson(string content, string[]? props)
    {
        if (string.IsNullOrWhiteSpace(content) || props is null || props.Length == 0)
        {
            return content;
        }

        string redacted = content;
        for (int i = 0; i < props.Length; i++)
        {
            string prop = Regex.Escape(props[i]);
            // "prop" : "value"  -> "prop":"***"
            string pattern = $"(\"{prop}\"\\s*:\\s*\")([^\"]*)(\")";
            redacted = Regex.Replace(redacted, pattern, $"$1***$3", RegexOptions.IgnoreCase);
        }
        return redacted;
    }

    public static string Truncate(string input, int max)
    {
        if (string.IsNullOrEmpty(input) || input.Length <= max)
        {
            return input ?? string.Empty;
        }
        return input.Substring(0, max) + "...(truncated)";
    }

    private static string Mask(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return string.Empty;
        }
        if (input.Length <= 8)
        {
            return "***";
        }
        return input.Substring(0, 4) + "..." + input.Substring(input.Length - 4);
    }
}