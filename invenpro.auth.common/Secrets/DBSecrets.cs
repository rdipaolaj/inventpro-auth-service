using System.Text.Json.Serialization;

namespace invenpro.auth.common.Secrets;

public class DBSecrets : ISecret
{
    [JsonPropertyName("username")]
    public string Username { get; set; } = string.Empty;

    [JsonPropertyName("password")]
    public string Password { get; set; } = string.Empty;

    [JsonPropertyName("host")]
    public string Host { get; set; } = string.Empty;

    [JsonPropertyName("port")]
    public int Port { get; set; }

    [JsonPropertyName("dbname")]
    public string DbName { get; set; } = string.Empty;

    [JsonPropertyName("dbClusterIdentifier")]
    public string DbClusterIdentifier { get; set; } = string.Empty;

    [JsonPropertyName("schemaplatform")]
    public string Schemaplatform { get; set; } = string.Empty;

    [JsonPropertyName("schemauser")]
    public string Schemauser { get; set; } = string.Empty;

    [JsonPropertyName("schemapayments")]
    public string Schemapayments { get; set; } = string.Empty;
}