using System.Security.Claims;
using System.Text;
using CatChatServer.Domain.Common.Settings;
using CatChatServer.Domain.Entities;
using CatChatServer.Domain.Interfaces;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Serilog;

namespace CatChatServer.Application.Services;

public class TokenService : ITokenService
{
    private readonly JwtSettings _jwtSettings;
    private readonly ILogger _logger;

    public TokenService(JwtSettings jwtSettings, ILogger logger)
    {
        _jwtSettings = jwtSettings;
        _logger = logger;
    }

    public string GenerateAccessToken(User user)
    {
        var tokenHandler = new JsonWebTokenHandler();

        byte[] key = Encoding.ASCII.GetBytes(_jwtSettings.Key);

        IEnumerable<Claim> claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience,
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationInMinutes),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256)
        };

        return tokenHandler.CreateToken(tokenDescriptor);
    }

    public string GenerateRefreshToken()
    {
        var tokenHandler = new JsonWebTokenHandler();

        byte[] key = Encoding.ASCII.GetBytes(_jwtSettings.Key);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience,
            Expires = DateTime.UtcNow.AddHours(24),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256)
        };

        return tokenHandler.CreateToken(tokenDescriptor);
    }

    public async Task<ClaimsIdentity?> GetPrincipalFromToken(string token)
    {
        byte[] key = Encoding.ASCII.GetBytes(_jwtSettings.Key);

        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidAudience = _jwtSettings.Audience,
            ValidIssuer = _jwtSettings.Issuer,
        };

        var tokenHandler = new JsonWebTokenHandler();

        TokenValidationResult principal = await tokenHandler.ValidateTokenAsync(token, tokenValidationParameters);
        if (principal.Exception is not null)
        {
            _logger.Error(principal.Exception.Message);
            throw new SecurityTokenException("invalid_token");
        }

        return principal.ClaimsIdentity;
    }

    public bool ValidateTokenExpired(string token)
    {
        if (string.IsNullOrEmpty(token))
        {
            return false;
        }

        var tokenHandler = new JsonWebTokenHandler();

        SecurityToken jwtToken = tokenHandler.ReadToken(token);

        if (jwtToken is null)
        {
            return false;
        }

        return jwtToken.ValidTo > DateTime.UtcNow;
    }
}
