using CatChatServer.Abstractions.BaseEntity;
using CatChatServer.Attributes;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CatChatServer.Entities;

[BsonCollection("Contacts")]
public class Contact : MongoEntity
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    [BsonElement("userId")]
    public virtual string UserId { get; set; } // Reference to the owner (User)
    
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    [BsonElement("contactUserId")]
    public virtual string ContactUserId { get; set; } // The other user in the chat
    
    [BsonElement("name")]
    public string Name { get; set; } = string.Empty; // Display name for the contact
    
    [BsonElement("lastMessage")]
    public string LastMessage { get; set; } = string.Empty; // Preview of the last message
    
    [BsonElement("unreadCount")]
    public int UnreadCount { get; set; } // Count of unread messages
}
