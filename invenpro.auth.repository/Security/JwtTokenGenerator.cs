using invenpro.auth.common.Settings;
using invenpro.auth.domain.AggregatesModel.UserAggregate;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace invenpro.auth.repository.Security;

public class JwtTokenGenerator(IConfiguration configuration, IOptions<JwtSettings> options) : IJwtTokenGenerator
{
    private readonly IConfiguration _configuration = configuration;
    private readonly JwtSettings _settings = options.Value;

    public string Generate(User user, string roleString)
    {
        string? secretKey = _settings.Secret;
        string? issuer = _configuration["Jwt:Issuer"];
        string? audience = _configuration["Jwt:Audience"];

        if (secretKey == null)
        {
            throw new InvalidOperationException("JWT Secret not configured.");
        }

        SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(secretKey));
        SigningCredentials credentials = new(key, SecurityAlgorithms.HmacSha256);

        List<Claim> claims = [];
        Claim idClaim = new(ClaimTypes.NameIdentifier, user.Id);
        Claim emailClaim = new(ClaimTypes.Email, user.Email);
        Claim nameClaim = new(ClaimTypes.Name, user.Name);
        Claim roleClaim = new(ClaimTypes.Role, roleString);

        claims.Add(idClaim);
        claims.Add(emailClaim);
        claims.Add(nameClaim);
        claims.Add(roleClaim);

        JwtSecurityToken token = new(
            issuer,
            audience,
            claims,
            DateTime.UtcNow,
            DateTime.UtcNow.AddHours(2),
            credentials);

        JwtSecurityTokenHandler handler = new();
        string tokenString = handler.WriteToken(token);
        return tokenString;
    }
}