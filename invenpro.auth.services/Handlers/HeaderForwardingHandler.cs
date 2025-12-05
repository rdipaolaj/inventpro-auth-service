using invenpro.auth.common.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace invenpro.auth.services.Handlers;

public sealed class HeaderForwardingHandler(IHttpContextAccessor httpContextAccessor) : DelegatingHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        HttpContext httpContext = _httpContextAccessor.HttpContext;

        if (httpContext is not null)
        {
            ForwardIfPresent(httpContext, request, HeaderConstant.ChannelCode);
            ForwardIfPresent(httpContext, request, HeaderConstant.TransactionId);
            ForwardIfPresent(httpContext, request, HeaderConstant.Timestamp);
        }

        HttpResponseMessage response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
        return response;
    }

    private static void ForwardIfPresent(HttpContext httpContext, HttpRequestMessage request, string headerName)
    {
        StringValues value;
        bool exists = httpContext.Request.Headers.TryGetValue(headerName, out value);
        if (exists)
        {
            bool alreadyPresent = request.Headers.Contains(headerName);
            if (!alreadyPresent)
            {
                request.Headers.Add(headerName, value.ToString());
            }
        }
    }
}