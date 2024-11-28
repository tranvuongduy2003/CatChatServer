using CatChatServer.Abstractions.BaseEntity;
using CatChatServer.Attributes;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CatChatServer.Entities;

[BsonCollection("Messages")]
public class Message : MongoEntity
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    [BsonElement("userId")]
    public virtual string ChatId { get; set; } // Reference to the chat
    
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    [BsonElement("userId")]
    public virtual string SenderId { get; set; } // Who sent the message
    
    [BsonElement("text")]
    public string Text { get; set; } = string.Empty; // Message content
    
    [BsonElement("mediaUrl")]
    public string? MediaUrl { get; set; } // Optional: URL for images, videos, or attachments
    
    [JsonConverter(typeof(StringEnumConverter))]
    [BsonRepresentation(BsonType.String)] 
    [BsonElement("senderType")]
    public ESenderType SenderType { get; set; } = ESenderType.User; // "user" or "other"
    
    [BsonElement("isRead")]
    public bool IsRead { get; set; } // Message read status
}

public enum ESenderType
{
    User = 0,
    Other = 1,
}
