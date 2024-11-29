using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CatChatServer.Domain.Common.Settings;
using CatChatServer.Domain.Entities;
using CatChatServer.Domain.Interfaces;
using CatChatServer.Domain.Repositories;
using Microsoft.IdentityModel.Tokens;

namespace CatChatServer.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly JwtSettings _jwtSettings;

    public AuthService(IUserRepository userRepository, IPasswordHasher passwordHasher, JwtSettings jwtSettings)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtSettings = jwtSettings;
    }

    public async Task<string?> AuthenticateUserAsync(string email, string password)
    {
        User user = await _userRepository.GetUserByEmailAsync(email);
        if (user == null || !_passwordHasher.VerifyPassword(password, user.PasswordHash))
        {
            return null;
        }

        return GenerateJwtToken(user);
    }

    private string GenerateJwtToken(User user)
    {
        var securityKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_jwtSettings.Key));
        var credentials = new SigningCredentials(
            securityKey, SecurityAlgorithms.HmacSha256);

        IEnumerable<Claim> claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(30),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
