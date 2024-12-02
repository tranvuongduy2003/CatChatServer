namespace CatChatServer.Domain.Common.BaseEntity;

public interface IMongoEntity
{
    string Id { get; protected init; }

    DateTime CreatedDate { get; set; }

    DateTime? LastModifiedDate { get; set; }
}
