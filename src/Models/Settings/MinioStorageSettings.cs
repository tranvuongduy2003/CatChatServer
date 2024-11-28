namespace CatChatServer.Models.Settings;

public sealed class MinioStorageSettings
{
    public string Endpoint { get; set; } = string.Empty;

    public string AccessKey { get; set; } = string.Empty;
    
    public string SecretKey { get; set; } = string.Empty;
    
    public bool Secure { get; set; }
}
