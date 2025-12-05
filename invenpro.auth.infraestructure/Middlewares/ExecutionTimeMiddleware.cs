using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace invenpro.auth.infraestructure.Middlewares;

public class ExecutionTimeMiddleware(RequestDelegate next, ILogger<ExecutionTimeMiddleware> logger)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<ExecutionTimeMiddleware> _logger = logger;
    private readonly List<string> Healchecks = ["/account-management", "/account-management/"];

    public async Task InvokeAsync(HttpContext context)
    {
        if (!Healchecks.Contains(context.Request.Path))
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            context.Response.OnStarting(state =>
            {
                HttpContext http = (HttpContext)state;
                Activity? act = Activity.Current;
                string tid = act is not null ? act.TraceId.ToString() : string.Empty;
                if (!string.IsNullOrEmpty(tid) && !http.Response.Headers.ContainsKey("trace-id"))
                {
                    http.Response.Headers["trace-id"] = tid;
                }
                return Task.CompletedTask;
            }, context);

            await _next(context);

            stopwatch.Stop();
            long elapsedTime = stopwatch.ElapsedMilliseconds;

            Activity? currentActivity = Activity.Current;
            string traceId = currentActivity is not null ? currentActivity.TraceId.ToString() : string.Empty;

            _logger.LogInformation(
                "Trace Request Performance : {Method} {Path} - StatusCode {StatusCode} | Time {ElapsedTime} ms | TraceId {TraceId}",
                context.Request.Method,
                context.Request.Path,
                context.Response.StatusCode,
                elapsedTime,
                traceId
            );
        }
        else
        {
            await _next(context);
        }
    }
}