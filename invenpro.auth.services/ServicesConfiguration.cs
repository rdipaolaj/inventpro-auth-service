using invenpro.auth.services.Handlers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Extensions.Http;

namespace invenpro.auth.services;

public static class ServicesConfiguration
{
    public static IServiceCollection AddServicesConfiguration(this IServiceCollection services)
    {
        services.AddTransient<HeaderForwardingHandler>();
        services.AddTransient<OutgoingRequestLoggingHandler>();
        services.AddTransient<HttpDeviceIpHeaderHandler>();


        return services;
    }

    #region Private methods

    private static HttpClientHandler GetCustomHttpClientHandler()
        => new()
        {
            AllowAutoRedirect = false,
            UseDefaultCredentials = true,
            ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; }
        };

    private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: attempt => TimeSpan.FromSeconds(1),
                onRetry: (outcome, timespan, retryAttempt, context) =>
                {
                    Console.WriteLine($"Reintento {retryAttempt} después de {timespan}. Error: {outcome.Exception?.Message ?? outcome.Result.StatusCode.ToString()}");
                });
    }

    #endregion
}