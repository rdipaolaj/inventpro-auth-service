using invenpro.auth.common.Constants;
using MediatR.Pipeline;
using Microsoft.AspNetCore.Http;

namespace invenpro.auth.infraestructure.PreProcessors;

public class CaptureHeadersPreProcessor<TRequest>(IHttpContextAccessor httpContextAccessor) : IRequestPreProcessor<TRequest>
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public Task Process(TRequest request, CancellationToken cancellationToken)
    {
        foreach (var header in ApiConstants.PropertyHeaders)
        {
            if (_httpContextAccessor.HttpContext.Request.Headers.TryGetValue(header, out var headerValue))
            {
                _httpContextAccessor.HttpContext.Items[header] = headerValue.ToString();
            }
        }

        return Task.CompletedTask;
    }
}