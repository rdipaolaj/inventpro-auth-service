using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Net;

namespace invenpro.auth.services;

internal abstract class BaseService(ILogger<BaseService> _logger)
{
    private readonly ILogger _logger = _logger;

    public async Task<HttpResponseMessage> SendAsync(HttpClient httpClient, HttpRequestMessage requestMessage)
    {
        if (httpClient is null)
        {
            _logger.LogError("HttpClient es null en SendAsync.");
            return new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent("HttpClient no puede ser null.")
            };
        }

        if (requestMessage is null)
        {
            _logger.LogError("HttpRequestMessage es null en SendAsync.");
            return new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent("RequestMessage no puede ser null.")
            };
        }

        if (requestMessage.RequestUri is null)
        {
            _logger.LogError("RequestUri es null en SendAsync.");
            return new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent("RequestUri no puede ser null.")
            };
        }

        string requestUri = FormatUrl(httpClient, requestMessage.RequestUri.ToString());

        try
        {
            return await ExecuteHttpRequest(httpClient, requestMessage, requestUri);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inesperado en SendAsync.");
            return new HttpResponseMessage(HttpStatusCode.InternalServerError)
            {
                Content = new StringContent($"Error interno: {ex.Message}")
            };
        }
    }

    #region Private methods

    private async Task<HttpResponseMessage> ExecuteHttpRequest(HttpClient httpClient, HttpRequestMessage requestMessage, string requestUri)
    {
        string httpMethod = requestMessage.Method.ToString();
        _logger.LogInformation("Iniciando request {HttpMethod} : {RequestUri}", httpMethod, requestUri);

        string requestBody = string.Empty;
        if (requestMessage.Content is not null)
        {
            requestBody = await requestMessage.Content.ReadAsStringAsync();
        }

        _logger.LogInformation("Request body : {RequestBody}", requestBody);

        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        HttpResponseMessage httpResponse;
        try
        {
            httpResponse = await httpClient.SendAsync(requestMessage);
            stopwatch.Stop();
            _logger.LogInformation("Request {HttpMethod}: {RequestUri}, Código {HttpStatusCode} - {StatusCode}, Duración {ElapsedMilliseconds} ms", httpMethod, requestUri, (int)httpResponse.StatusCode, httpResponse.StatusCode, stopwatch.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "Error Request {HttpMethod} {RequestUri}, Duración {ElapsedMilliseconds} ms, Excepción : {Message}", httpMethod, requestUri, stopwatch.ElapsedMilliseconds, ex.Message);
            return new HttpResponseMessage(HttpStatusCode.BadGateway)
            {
                Content = new StringContent($"Error al llamar al servicio: {ex.Message}")
            };
        }

        string responseBody = string.Empty;
        if (httpResponse.Content is not null)
        {
            responseBody = await httpResponse.Content.ReadAsStringAsync();
        }

        _logger.LogInformation("Response body : {ResponseBody}", responseBody);

        return httpResponse;
    }
    private static string FormatUrl(HttpClient httpClient, string requestUri)
    {
        if (httpClient.BaseAddress is null)
        {
            return requestUri;
        }

        return httpClient.BaseAddress.ToString() + requestUri;
    }

    #endregion
}