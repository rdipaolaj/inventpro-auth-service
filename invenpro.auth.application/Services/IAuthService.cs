using invenpro.auth.common.Responses;
using invenpro.auth.dto.Auth;

namespace invenpro.auth.application.Services;

public interface IAuthService
{
    Task<ApiResponse<LoginResponse>> LoginAsync(string email, string password, CancellationToken cancellationToken = default);

    Task<ApiResponse<LogoutResponse>> LogoutAsync(string userId, CancellationToken cancellationToken = default);
}