namespace CatChatServer.Domain.Interfaces;

public interface IAuthService
{
    Task<string?> AuthenticateUserAsync(string email, string password);
}
