using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace invenpro.auth.common.Helper;

public class NullableDateTimeConverter : JsonConverter<DateTime?>
{
    public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            string? s = reader.GetString();
            if (string.IsNullOrWhiteSpace(s))
                return null;
            return DateTime.Parse(s, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal);
        }
        if (reader.TokenType == JsonTokenType.Null)
            return null;
        return reader.GetDateTime();
    }

    public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
    {
        if (value.HasValue)
            writer.WriteStringValue(value.Value.ToString("o"));
        else
            writer.WriteNullValue();
    }
}