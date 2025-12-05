using invenpro.auth.common.Constants;

namespace invenpro.auth.common.Settings;

public sealed class Observability
{
    public string? ServiceName { get; set; }
    public OtlpSettings? Otlp { get; set; }
    public string? Sampling { get; set; }
    public string[]? IgnorePaths { get; set; }
    public bool EnableRedis { get; set; }
    public HttpLoggingSettings? HttpLogging { get; set; }
}

public sealed class OtlpSettings
{
    public string? Endpoint { get; set; }
    public string? Protocol { get; set; }
}

public sealed class HttpLoggingSettings
{
    public bool Enabled { get; set; } = true;

    // ===== Lectura request =====
    public int MaxRequestBodyBytes { get; set; } = HttpLoggingDefaults.DefaultMaxRequestBodyBytes;

    // ===== Lectura response =====
    public int MaxResponseBodyBytes { get; set; } = HttpLoggingDefaults.DefaultMaxResponseBodyBytes;

    public bool BufferAuthenticatedResponses { get; set; } = HttpLoggingDefaults.DefaultBufferAuthenticatedResponses;

    public int BufferResponsesAtOrAboveStatus { get; set; } = HttpLoggingDefaults.DefaultBufferResponsesAtOrAboveStatus;

    public bool AttachBodiesToSpans { get; set; } = HttpLoggingDefaults.DefaultAttachBodiesToSpans;

    // Sanitizado
    public int MaxBodyLength { get; set; } = HttpLoggingDefaults.DefaultMaxBodyLengthToSanitize;

    // Listas
    public string[]? IgnorePaths { get; set; }
    public string[]? SensitiveHeaders { get; set; }
    public string[]? SensitiveJsonProps { get; set; }
    public string[]? ContentTypes { get; set; }
}