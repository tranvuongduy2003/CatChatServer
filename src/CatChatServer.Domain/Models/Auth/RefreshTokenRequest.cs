namespace CatChatServer.Domain.Models.Auth;

public class RefreshTokenRequest
{
    public RefreshTokenRequest()
    {
    }

    public RefreshTokenRequest(string accessToken, string refreshToken)
    {
        AccessToken = accessToken;
        RefreshToken = refreshToken;
    }

    public string AccessToken { get; set; }

    public string RefreshToken { get; set; }
}
