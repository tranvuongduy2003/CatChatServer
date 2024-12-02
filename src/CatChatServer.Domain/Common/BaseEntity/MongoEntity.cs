using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CatChatServer.Domain.Common.BaseEntity;

public abstract class MongoEntity : IMongoEntity
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    [BsonElement("_id")]
    public virtual string Id { get; init; }

    [BsonElement("createdDate")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    [BsonElement("lastModifiedDate")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime? LastModifiedDate { get; set; }
}
