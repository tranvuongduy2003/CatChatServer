using CatChatServer.API.GraphQL.Types;
using CatChatServer.Domain.Interfaces;
using CatChatServer.Domain.Models.Auth;
using HotChocolate;
using HotChocolate.Authorization;
using HotChocolate.Types;

namespace CatChatServer.API.GraphQL.Mutations;

[ExtendObjectType(typeof(MutationBaseType))]
public sealed class AuthMutations
{
    public async Task<AuthResponse> Register(
        [Service] IAuthService authService,
        string username,
        string email,
        string password)
    {
        AuthResponse authResponse = await authService.Register(new RegisterRequest(username, email, password));
        return authResponse;
    }

    public async Task<AuthResponse> Login(
        [Service] IAuthService authService,
        string email,
        string password)
    {
        AuthResponse authResponse = await authService.Login(new LoginRequest(email, password));
        return authResponse;
    }

    [Authorize]
    public async Task<AuthResponse> RefreshToken(
        [Service] IAuthService authService,
        string accessToken,
        string refreshToken)
    {
        AuthResponse authResponse = await authService.RefreshToken(new RefreshTokenRequest(accessToken, refreshToken));
        return authResponse;
    }
}
