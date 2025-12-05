namespace invenpro.auth.common.Services;

public interface IContextAccessorService
{
    string GetClaim(string claimId);
    string GetContextItem(string key);
}