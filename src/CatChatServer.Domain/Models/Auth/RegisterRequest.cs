namespace CatChatServer.Domain.Models.Auth;

public class RegisterRequest
{
    public RegisterRequest()
    {
    }

    public RegisterRequest(string username, string email, string password)
    {
        Username = username;
        Email = email;
        Password = password;
    }

    public string Username { get; set; }

    public string Email { get; set; }

    public string Password { get; set; }
}
