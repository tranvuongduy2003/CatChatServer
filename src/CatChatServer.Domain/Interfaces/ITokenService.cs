using System.Security.Claims;
using CatChatServer.Domain.Entities;

namespace CatChatServer.Domain.Interfaces;

public interface ITokenService
{
    string GenerateAccessToken(User user);

    string GenerateRefreshToken();

    Task<ClaimsIdentity?> GetPrincipalFromToken(string token);

    bool ValidateTokenExpired(string token);
}
