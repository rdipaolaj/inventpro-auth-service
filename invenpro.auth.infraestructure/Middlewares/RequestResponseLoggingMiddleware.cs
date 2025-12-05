using invenpro.auth.common.Constants;
using invenpro.auth.common.Logging;
using invenpro.auth.common.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Buffers;
using System.Diagnostics;
using System.Text;

namespace invenpro.auth.infraestructure.Middlewares;

public sealed class RequestResponseLoggingMiddleware(RequestDelegate next, ILogger<RequestResponseLoggingMiddleware> logger, IOptions<Observability> obs)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<RequestResponseLoggingMiddleware> _logger = logger;
    private readonly HttpLoggingSettings _settings = obs.Value.HttpLogging ?? new HttpLoggingSettings();

    public async Task InvokeAsync(HttpContext context)
    {
        if (!_settings.Enabled || ShouldIgnore(context.Request.Path))
        {
            await _next(context);
            return;
        }

        string traceId = Activity.Current != null ? Activity.Current.TraceId.ToString() : string.Empty;

        string requestBodyClean = string.Empty;
        IDictionary<string, string> requestHeaders =
            HttpLoggingSanitizer.RedactHeaders(context.Request.Headers.AsEnumerable(), _settings.SensitiveHeaders);

        if (MethodMayHaveBody(context.Request.Method) && ShouldCaptureBody(context.Request.ContentType))
        {
            string rawBody = await ReadRequestBodyCappedAsync(context.Request, _settings.MaxRequestBodyBytes);
            requestBodyClean = HttpLoggingSanitizer.Truncate(
                HttpLoggingSanitizer.RedactJson(rawBody, _settings.SensitiveJsonProps),
                _settings.MaxBodyLength);

            if (_settings.AttachBodiesToSpans && Activity.Current is not null)
            {
                Activity.Current.SetTag("http.request.body_preview", requestBodyClean);
            }
        }

        // ===== Política de buferizado =====
        bool isAuthenticated = context.User != null && context.User.Identity != null && context.User.Identity.IsAuthenticated;
        bool allowAuthBuffer = _settings.BufferAuthenticatedResponses && isAuthenticated;

        bool bufferDueToPolicy = allowAuthBuffer;

        Stream originalBody = context.Response.Body;
        MemoryStream? tempBody = null;

        try
        {
            if (bufferDueToPolicy)
            {
                tempBody = new MemoryStream(capacity: _settings.MaxResponseBodyBytes);
                context.Response.Body = tempBody;
            }

            await _next(context);

            Activity? act = Activity.Current;

            if (tempBody is null)
            {
                if (act is not null && context.Response.StatusCode >= 400)
                {
                    act.SetTag(OtelTag.HttpResponseStatusCode, context.Response.StatusCode);
                    act.SetStatus(ActivityStatusCode.Error);
                    act.SetTag(OtelTag.ErrorType, context.Response.StatusCode >= 500 ? "ServerError" : "ClientError");
                    act.SetTag(OtelTag.ErrorMessage, "HTTP " + context.Response.StatusCode.ToString());
                }

                _logger.LogInformation(
                    "INBOUND | {Method} {Path} | Query {Query} | TraceId {TraceId} | Status {StatusCode} | ReqHeaders {@ReqHeaders}",
                    context.Request.Method,
                    context.Request.Path,
                    context.Request.QueryString.HasValue ? context.Request.QueryString.Value : string.Empty,
                    traceId,
                    context.Response.StatusCode,
                    requestHeaders
                );

                return;
            }

            // ===== Response (buffered con límites) =====
            bool mustLogBody = true;

            if (context.Response.StatusCode < _settings.BufferResponsesAtOrAboveStatus && !allowAuthBuffer)
            {
                mustLogBody = false;
            }

            if (mustLogBody)
            {
                string responseBodyClean = string.Empty;

                if (tempBody.Length <= _settings.MaxResponseBodyBytes)
                {
                    context.Response.Body.Seek(0, SeekOrigin.Begin);
                    using StreamReader reader = new(context.Response.Body, Encoding.UTF8, true, 4096, true);
                    string responseText = await reader.ReadToEndAsync();
                    context.Response.Body.Seek(0, SeekOrigin.Begin);

                    IDictionary<string, string> responseHeaders =
                        HttpLoggingSanitizer.RedactHeaders(context.Response.Headers.AsEnumerable(), _settings.SensitiveHeaders);

                    responseBodyClean = HttpLoggingSanitizer.Truncate(
                        HttpLoggingSanitizer.RedactJson(responseText, _settings.SensitiveJsonProps),
                        _settings.MaxBodyLength);

                    _logger.LogInformation(
                        "INBOUND | {Method} {Path} | Query {Query} | Route {@RouteValues} | TraceId {TraceId} | Status {StatusCode} | ReqHeaders {@ReqHeaders} | ReqBody {ReqBody} | ResHeaders {@ResHeaders} | ResBody {ResBody}",
                        context.Request.Method,
                        context.Request.Path,
                        context.Request.QueryString.HasValue ? context.Request.QueryString.Value : string.Empty,
                        context.GetRouteData()?.Values,
                        traceId,
                        context.Response.StatusCode,
                        requestHeaders,
                        requestBodyClean,
                        responseHeaders,
                        responseBodyClean
                    );

                    if (act is not null)
                    {
                        if (_settings.AttachBodiesToSpans)
                        {
                            act.SetTag(OtelTag.HttpResponseBodyPreview, responseBodyClean);
                        }

                        if (context.Response.StatusCode >= 400)
                        {
                            act.SetTag(OtelTag.HttpResponseStatusCode, context.Response.StatusCode);
                            act.SetStatus(ActivityStatusCode.Error);
                            act.SetTag(OtelTag.ErrorType, context.Response.StatusCode >= 500 ? "ServerError" : "ClientError");
                            act.SetTag(OtelTag.ErrorMessage, "HTTP " + context.Response.StatusCode.ToString());
                            act.SetTag(OtelTag.ErrorDetail, responseBodyClean);
                        }
                    }
                }
                else
                {
                    const string responseBodyNote = "(response body omitted: too large)";

                    _logger.LogInformation(
                        "INBOUND | {Method} {Path} | Query {Query} | TraceId {TraceId} | Status {StatusCode} | ReqHeaders {@ReqHeaders} | ReqBody {ReqBody} | ResBody {ResBodyNote}",
                        context.Request.Method,
                        context.Request.Path,
                        context.Request.QueryString.HasValue ? context.Request.QueryString.Value : string.Empty,
                        traceId,
                        context.Response.StatusCode,
                        requestHeaders,
                        requestBodyClean,
                        responseBodyNote
                    );

                    if (act is not null && context.Response.StatusCode >= 400)
                    {
                        act.SetTag(OtelTag.HttpResponseStatusCode, context.Response.StatusCode);
                        act.SetStatus(ActivityStatusCode.Error);
                        act.SetTag(OtelTag.ErrorType, context.Response.StatusCode >= 500 ? "ServerError" : "ClientError");
                        act.SetTag(OtelTag.ErrorMessage, "HTTP " + context.Response.StatusCode.ToString());
                    }
                }
            }

            if (!context.Response.HasStarted)
            {
                context.Response.ContentLength = null;
            }

            tempBody.Seek(0, SeekOrigin.Begin);
            await tempBody.CopyToAsync(originalBody);
        }
        finally
        {
            context.Response.Body = originalBody;
            if (tempBody != null)
            {
                tempBody.Dispose();
            }
        }
    }

    private static bool MethodMayHaveBody(string method)
    {
        return string.Equals(method, "POST", StringComparison.OrdinalIgnoreCase)
            || string.Equals(method, "PUT", StringComparison.OrdinalIgnoreCase)
            || string.Equals(method, "PATCH", StringComparison.OrdinalIgnoreCase)
            || string.Equals(method, "DELETE", StringComparison.OrdinalIgnoreCase);
    }

    private bool ShouldIgnore(PathString path)
    {
        if (_settings.IgnorePaths is null || _settings.IgnorePaths.Length == 0)
        {
            return false;
        }

        for (int i = 0; i < _settings.IgnorePaths.Length; i++)
        {
            string? ignore = _settings.IgnorePaths[i];
            if (!string.IsNullOrWhiteSpace(ignore) &&
                path.StartsWithSegments(ignore, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }
        return false;
    }

    private bool ShouldCaptureBody(string? contentType)
    {
        if (_settings.ContentTypes is null || string.IsNullOrWhiteSpace(contentType))
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

    private static async Task<string> ReadRequestBodyCappedAsync(HttpRequest request, int maxBytes)
    {
        if (request.Body is null)
        {
            return string.Empty;
        }

#if NET8_0_OR_GREATER
        request.EnableBuffering(bufferThreshold: HttpLoggingDefaults.DefaultReadBufferThreshold, bufferLimit: maxBytes);
#else
            request.EnableBuffering();
#endif
        request.Body.Seek(0, SeekOrigin.Begin);

        int remaining = maxBytes;
        StringBuilder sb = new(Math.Min(maxBytes, 4096));
        char[] rented = ArrayPool<char>.Shared.Rent(4096);
        try
        {
            using StreamReader reader = new(request.Body, Encoding.UTF8, true, 4096, true);
            while (remaining > 0)
            {
                int toRead = Math.Min(4096, remaining);
                int read = await reader.ReadAsync(rented, 0, toRead);
                if (read <= 0)
                {
                    break;
                }
                sb.Append(rented, 0, read);
                remaining -= read;
            }
        }
        finally
        {
            ArrayPool<char>.Shared.Return(rented);
            request.Body.Seek(0, SeekOrigin.Begin);
        }

        return sb.ToString();
    }
}