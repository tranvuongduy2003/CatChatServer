using CatChatServer.Domain.Attributes;
using CatChatServer.Domain.Common;
using CatChatServer.Domain.Common.BaseEntity;
using CatChatServer.Domain.Common.Constants;
using CatChatServer.Domain.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CatChatServer.Domain.Entities;

[BsonCollection(CollectionName.Users)]
public class User : MongoEntity
{
    [BsonElement("username")] public string Username { get; set; } = string.Empty;

    [BsonElement("email")] public string Email { get; set; } = string.Empty;

    [BsonElement("passwordHash")] public string PasswordHash { get; set; } = string.Empty;

    [BsonElement("profilePicture")] public string? ProfilePicture { get; set; } // Optional profile picture URL

    [BsonElement("refreshToken")] public string? RefreshToken { get; set; } // For JWT refresh token

    [BsonElement("lastLoginAt")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime? LastLoginAt { get; set; }

    [BsonElement("settings")] public UserSettings Settings { get; set; } = new UserSettings();

    [JsonConverter(typeof(StringEnumConverter))]
    [BsonRepresentation(BsonType.String)]
    [BsonElement("status")]
    public EUserStatus Status { get; set; } = EUserStatus.Offline;
}

public class UserSettings
{
    [BsonElement("privacyMode")] public bool PrivacyMode { get; set; } // Hide status, last seen, etc.

    [BsonElement("notificationsEnabled")]
    public bool NotificationsEnabled { get; set; } = true; // Enable/disable notifications

    [BsonElement("darkMode")] public bool DarkMode { get; set; } // Toggle dark mode

    [BsonElement("language")] public string Language { get; set; } = "en"; // Preferred language
}
