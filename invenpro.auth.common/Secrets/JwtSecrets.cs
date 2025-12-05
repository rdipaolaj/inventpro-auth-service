using System.Text.Json.Serialization;

namespace invenpro.auth.common.Secrets;

public class JwtSecrets : ISecret
{
    [JsonPropertyName("secret")]
    public string Secret { get; set; } = string.Empty;
}