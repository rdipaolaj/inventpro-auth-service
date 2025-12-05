using invenpro.auth.common.Constants;
using invenpro.auth.common.Responses;
using invenpro.auth.domain.AggregatesModel.UserAggregate;
using invenpro.auth.dto.Auth;
using invenpro.auth.repository.Security;
using Microsoft.AspNetCore.Http;

namespace invenpro.auth.application.Services.Implementations;

internal sealed class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public AuthService(IUserRepository userRepository, IJwtTokenGenerator jwtTokenGenerator)
    {
        _userRepository = userRepository;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<ApiResponse<LoginResponse>> LoginAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        ApiResponse<LoginResponse> response = new ApiResponse<LoginResponse>();

        User? user = await _userRepository.GetByEmailAsync(email, cancellationToken);
        if (user == null)
        {
            return response.WithError(AuthMessageCode.InvalidCredentials, AuthMessageDescription.InvalidCredentials, StatusCodes.Status401Unauthorized);
        }

        bool passwordValid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
        if (!passwordValid)
        {
            return response.WithError(AuthMessageCode.InvalidCredentials, AuthMessageDescription.InvalidCredentials, StatusCodes.Status401Unauthorized);
        }

        string roleString = ConvertRoleToString(user.Role);
        string token = _jwtTokenGenerator.Generate(user, roleString);

        AuthUserResponse userResponse = new AuthUserResponse();
        userResponse.Id = user.Id;
        userResponse.Email = user.Email;
        userResponse.Name = user.Name;
        userResponse.Role = roleString;
        userResponse.Avatar = user.Avatar;
        userResponse.CreatedAt = user.CreatedAt;
        userResponse.UpdatedAt = user.UpdatedAt;

        LoginResponse loginResponse = new LoginResponse();
        loginResponse.User = userResponse;
        loginResponse.Token = token;

        return response.WithSuccess(loginResponse, StatusCodes.Status200OK);
    }

    public Task<ApiResponse<LogoutResponse>> LogoutAsync(string userId, CancellationToken cancellationToken = default)
    {
        ApiResponse<LogoutResponse> response = new ApiResponse<LogoutResponse>();

        LogoutResponse data = new LogoutResponse();
        data.LoggedOut = true;

        return Task.FromResult(response.WithSuccess(data, StatusCodes.Status200OK));
    }

    private static string ConvertRoleToString(UserRole role)
    {
        if (role == UserRole.Admin)
        {
            return "admin";
        }

        if (role == UserRole.Manager)
        {
            return "manager";
        }

        if (role == UserRole.Employee)
        {
            return "employee";
        }

        throw new InvalidOperationException("Unknown role.");
    }
}