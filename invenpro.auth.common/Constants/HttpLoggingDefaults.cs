namespace invenpro.auth.common.Constants;

public static class HttpLoggingDefaults
{
    // LÍMITES por defecto
    public const int DefaultMaxRequestBodyBytes = 32 * 1024;
    public const int DefaultMaxResponseBodyBytes = 64 * 1024;
    public const int DefaultReadBufferThreshold = 64 * 1024;

    // Comportamientos por defecto
    public const bool DefaultBufferAuthenticatedResponses = true;
    public const int DefaultBufferResponsesAtOrAboveStatus = 500;
    public const bool DefaultAttachBodiesToSpans = false;

    // Sanitizado
    public const int DefaultMaxBodyLengthToSanitize = 2048;
}