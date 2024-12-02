using CatChatServer.Domain.Models.Auth;

namespace CatChatServer.Domain.Interfaces;

public interface IAuthService
{
    Task<AuthResponse> Login(LoginRequest request);

    Task<AuthResponse> Register(RegisterRequest request);

    Task<AuthResponse> RefreshToken(RefreshTokenRequest request);
}
