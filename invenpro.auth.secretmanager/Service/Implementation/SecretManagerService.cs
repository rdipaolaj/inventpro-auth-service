using Amazon;
using Amazon.Runtime;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using invenpro.auth.common.Constants;
using invenpro.auth.common.Secrets;
using invenpro.auth.common.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Text.Json;

namespace invenpro.auth.secretmanager.Service.Implementation;

public sealed class SecretManagerService : ISecretManagerService
{
    private readonly AmazonSecretsManagerClient _client;
    private readonly ILogger<SecretManagerService> _logger;
    private readonly IOptions<EnvironmentSettings> _environmentSettings;

    public SecretManagerService(ILogger<SecretManagerService> logger, IOptions<SecretManagerSettings> options, IOptions<EnvironmentSettings> environmentSettings)
    {
        _logger = logger;

        if (options.Value.UseLocalstack)
        {
            _client = new AmazonSecretsManagerClient
            (
                new BasicAWSCredentials(options.Value.AWSLocalStackKey, options.Value.AWSLocalStackSecret),
                new AmazonSecretsManagerConfig
                {
                    ServiceURL = options.Value.LocalStackServiceURL
                }
            );
        }
        else
        {
            _client = new(RegionEndpoint.GetBySystemName(options.Value.Region));
        }

        _environmentSettings = environmentSettings;
    }

    public async Task<T?> GetSecret<T>(string arn) where T : ISecret
    {
        T? result = default;
        Stopwatch stopwatch = new();
        stopwatch.Start();

        _logger.LogInformation("Obteniendo valores de secret manager con Arn {arn}", arn);

        try
        {
            GetSecretValueResponse response = await _client.GetSecretValueAsync(new GetSecretValueRequest { SecretId = arn });
            if (_environmentSettings.Value.Environment is not EnvironmentConstants.Production)
                _logger.LogInformation("Obteniendo response: {response}", response.SecretString);

            if (response.SecretString is not null)
            {
                result = JsonSerializer.Deserialize<T>(response.SecretString);
            }
            else
            {
                _logger.LogWarning("El secreto obtenido con ARN {arn} no contiene ningún valor.", arn);
            }

            stopwatch.Stop();
            _logger.LogInformation("Valores obtenidos de Arn {arn} satisfactorios, Duración ms : {ElapsedMilliseconds}", arn, stopwatch.ElapsedMilliseconds);
        }
        catch (JsonException jsonEx)
        {
            _logger.LogError("Error de deserialización: {Message}", jsonEx.Message);
        }
        catch (AmazonSecretsManagerException e)
        {
            _logger.LogError("Error al obtener el secreto {arn}: {Message}", arn, e.Message);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError("Error al obtener valores de secret manager con Arn {arn}, Duración ms : {ElapsedMilliseconds}, Error : {Message}", arn, stopwatch.ElapsedMilliseconds, ex.Message);
        }

        return result;
    }
}