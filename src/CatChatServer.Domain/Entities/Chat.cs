using CatChatServer.Domain.Attributes;
using CatChatServer.Domain.Common.BaseEntity;
using CatChatServer.Domain.Common.Constants;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CatChatServer.Domain.Entities;

[BsonCollection(CollectionName.Chats)]
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
