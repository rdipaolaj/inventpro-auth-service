using OpenTelemetry.Exporter;

namespace invenpro.auth.common.Constants;

public static class TelemetryConstants
{
    public const string ActivitySourceName = "banbif.pnbxi.account.management.channel";
}

public static class OtelTag
{
    // OTel semantic conventions
    public const string HttpRequestMethod = "http.request.method";
    public const string UrlFull = "url.full";
    public const string HttpResponseStatusCode = "http.response.status_code";
    public const string HttpResponseLength = "http.response.length";
    public const string HttpResponseContentType = "http.response_content_type";
    public const string HttpRequestTraceIdentifier = "http.request_trace_identifier";

    // Preview de cuerpos
    public const string HttpRequestBodyPreview = "http.request.body_preview";
    public const string HttpResponseBodyPreview = "http.response.body_preview";

    // Excepciones
    public const string ExceptionType = "exception.type";
    public const string ExceptionMessage = "exception.message";
    public const string ExceptionStacktrace = "exception.stacktrace";
    public const string ErrorType = "error.type";
    public const string ErrorMessage = "error.message";
    public const string ErrorDetail = "error.detail";

    // Dominio
    public const string BanbifTransactionId = "banbif.transaction_id";
    public const string BanbifChannelCode = "banbif.channel_code";

}

public static class OtelResAttr
{
    public const string DeploymentEnvironment = "deployment.environment";
}

public static class OtelDefaults
{
    public const string DefaultServiceName = "account-management-bxi";
    public const string DefaultOtlpEndpoint = "http://localhost:4317";
    public const string DefaultSamplingMode = "always_on";
    public static readonly OtlpExportProtocol DefaultOtlpProtocol = OtlpExportProtocol.Grpc;
}

public static class OtelProtocolNames
{
    public const string Grpc = "grpc";
    public const string Http = "http"; // HttpProtobuf
}

public static class SamplingModes
{
    public const string AlwaysOn = "always_on";
    public const string AlwaysOff = "always_off";
    public const string On = "on";
    public const string Off = "off";
    public const string RatioPref = "ratio:"; // e.g. ratio:0.25
}

public static class LogTemplates
{
    public const string Inbound =
        "INBOUND | {Method} {Path} | Query {Query} | Route {@RouteValues} | TraceId {TraceId} | Status {StatusCode} | ReqHeaders {@ReqHeaders} | ReqBody {ReqBody} | ResHeaders {@ResHeaders} | ResBody {ResBody}";

    public const string Outbound =
        "OUTBOUND | {Method} {Url} | TraceId {TraceId} | Status {StatusCode} | ReqHeaders {@ReqHeaders} | ReqBody {ReqBody} | ResHeaders {@ResHeaders} | ResBody {ResBody}";
}