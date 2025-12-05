namespace invenpro.auth.dto.Auth;

public class LoginResponse
{
    public AuthUserResponse User { get; set; } = new AuthUserResponse();

    public string Token { get; set; } = string.Empty;
}