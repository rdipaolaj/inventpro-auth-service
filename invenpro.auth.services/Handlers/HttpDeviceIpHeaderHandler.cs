using invenpro.auth.common.Constants;
using invenpro.auth.common.Services;
using invenpro.auth.common.Validations;

namespace invenpro.auth.services.Handlers;

internal class HttpDeviceIpHeaderHandler(IContextAccessorService contextAccessorService) : DelegatingHandler
{
    private readonly IContextAccessorService _contextAccessorService = contextAccessorService;

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        string deviceIp = _contextAccessorService.GetContextItem(HeaderConstant.DeviceIp);

        if (!string.IsNullOrEmpty(deviceIp) && RegexValidator.NoIpv4().IsMatch(deviceIp))
        {
            request.Headers.Add(AuthConstants.AuthForwardedForHeader, deviceIp);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}