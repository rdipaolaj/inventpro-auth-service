using invenpro.auth.common.Logging;
using invenpro.auth.common.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using System.Diagnostics;

namespace invenpro.auth.services.Handlers;

public sealed class OutgoingRequestLoggingHandler(ILogger<OutgoingRequestLoggingHandler> logger, IOptions<Observability> obs) : DelegatingHandler
{
    private readonly ILogger<OutgoingRequestLoggingHandler> _logger = logger;
    private readonly HttpLoggingSettings _settings = obs.Value.HttpLogging ?? new HttpLoggingSettings();

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (!_settings.Enabled)
        {
            return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
        }

        string traceId = Activity.Current is not null ? Activity.Current.TraceId.ToString() : string.Empty;

        string requestBody = string.Empty;
        if (request.Content is not null && ShouldCaptureBody(request.Content.Headers.ContentType is not null ? request.Content.Headers.ContentType.MediaType : null))
        {
            requestBody = await request.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        }

        IDictionary<string, string> requestHeaders = HttpLoggingSanitizer.RedactHeaders(request.Headers.ToDictionary(h => h.Key, h => new StringValues(h.Value.ToArray())), _settings.SensitiveHeaders);
        requestBody = HttpLoggingSanitizer.RedactJson(requestBody, _settings.SensitiveJsonProps);
        requestBody = HttpLoggingSanitizer.Truncate(requestBody, _settings.MaxBodyLength);

        HttpResponseMessage response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);

        string responseBody = string.Empty;
        if (response.Content is not null && ShouldCaptureBody(response.Content.Headers.ContentType is not null ? response.Content.Headers.ContentType.MediaType : null))
        {
            responseBody = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        }
        IDictionary<string, string> responseHeaders = HttpLoggingSanitizer.RedactHeaders(response.Headers.ToDictionary(h => h.Key, h => new StringValues(h.Value.ToArray())), _settings.SensitiveHeaders);
        responseBody = HttpLoggingSanitizer.RedactJson(responseBody, _settings.SensitiveJsonProps);
        responseBody = HttpLoggingSanitizer.Truncate(responseBody, _settings.MaxBodyLength);

        _logger.LogInformation("OUTBOUND | {Method} {Url} | TraceId {TraceId} | Status {StatusCode} | ReqHeaders {@ReqHeaders} | ReqBody {ReqBody} | ResHeaders {@ResHeaders} | ResBody {ResBody}",
            request.Method,
            request.RequestUri,
            traceId,
            (int)response.StatusCode,
            requestHeaders,
            requestBody,
            responseHeaders,
            responseBody);

        if (_settings.AttachBodiesToSpans && Activity.Current is not null)
        {
            Activity.Current.SetTag("http.client.request.body_preview", requestBody);
            Activity.Current.SetTag("http.client.response.body_preview", responseBody);
        }

        return response;
    }

    private bool ShouldCaptureBody(string? contentType)
    {
        if (_settings.ContentTypes == null || string.IsNullOrWhiteSpace(contentType))
        {
            return false;
        }
        for (int i = 0; i < _settings.ContentTypes.Length; i++)
        {
            if (contentType.StartsWith(_settings.ContentTypes[i], StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }
        return false;
    }
}