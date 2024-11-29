namespace CatChatServer.Domain.Common.Settings;

public sealed class MongoDBSettings
{
    public string Host { get; set; } = string.Empty;
    
    public int Port { get; set; }

    public string DatabaseName { get; set; } = string.Empty;

    public string User { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;
}
