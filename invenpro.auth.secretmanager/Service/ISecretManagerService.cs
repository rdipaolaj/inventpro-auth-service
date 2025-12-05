using invenpro.auth.common.Secrets;

namespace invenpro.auth.secretmanager.Service;

public interface ISecretManagerService
{
    Task<T?> GetSecret<T>(string arn) where T : ISecret;
}