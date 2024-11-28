using CatChatServer.Abstractions.BaseEntity;
using CatChatServer.Attributes;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CatChatServer.Entities;

[BsonCollection("Chats")]
public class Chat : MongoEntity
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    [BsonElement("user1Id")]
    public virtual string User1Id { get; set; } // First user in the chat
    
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    [BsonElement("user2Id")]
    public virtual string User2Id { get; set; } // Second user in the chat
    
    public ICollection<Message> Messages { get; set; } = new List<Message>();
}
