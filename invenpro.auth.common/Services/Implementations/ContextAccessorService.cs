using Microsoft.AspNetCore.Http;

namespace invenpro.auth.common.Services.Implementations;

internal sealed class ContextAccessorService(IHttpContextAccessor httpContextAccessor) : IContextAccessorService
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public string GetClaim(string claimId)
        => _httpContextAccessor.HttpContext?
                .User?
                .FindFirst(claimId)?
                .Value
           ?? string.Empty;

    public string GetContextItem(string key)
        => _httpContextAccessor.HttpContext?
                .Items[key]?
                .ToString()
           ?? string.Empty;
}