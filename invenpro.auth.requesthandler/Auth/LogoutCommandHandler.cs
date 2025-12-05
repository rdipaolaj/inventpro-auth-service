using invenpro.auth.application.Services;
using invenpro.auth.common.Responses;
using invenpro.auth.dto.Auth;
using invenpro.auth.request.Auth.Commands;
using MediatR;

namespace invenpro.auth.requesthandler.Auth;

internal sealed class LogoutCommandHandler : IRequestHandler<LogoutCommand, ApiResponse<LogoutResponse>>
{
    private readonly IAuthService _authService;

    public LogoutCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<ApiResponse<LogoutResponse>> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        ApiResponse<LogoutResponse> response = await _authService.LogoutAsync(request.UserId, cancellationToken);
        return response;
    }
}