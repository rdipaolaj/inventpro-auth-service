using invenpro.auth.application.Services;
using invenpro.auth.common.Responses;
using invenpro.auth.dto.Auth;
using invenpro.auth.request.Auth.Commands;
using MediatR;

namespace invenpro.auth.requesthandler.Auth;

internal sealed class LoginCommandHandler : IRequestHandler<LoginCommand, ApiResponse<LoginResponse>>
{
    private readonly IAuthService _authService;

    public LoginCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<ApiResponse<LoginResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        ApiResponse<LoginResponse> response = await _authService.LoginAsync(request.Email, request.Password, cancellationToken);
        return response;
    }
}