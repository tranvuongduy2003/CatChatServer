using CatChatServer.Domain.Common.Constants;
using CatChatServer.Domain.Common.Settings;
using CatChatServer.Domain.Entities;
using MongoDB.Driver;
using MongoDB.Driver.Core.Configuration;

namespace CatChatServer.Infrastructure.Data;

public class MongoDBContext
{
    private readonly IMongoDatabase _database;

    public MongoDBContext(IMongoClient mongoClient, MongoDBSettings mongoDBSettings)
    {
        _database = mongoClient.GetDatabase(mongoDBSettings.DatabaseName);
    }

    public IMongoCollection<User> Users => _database.GetCollection<User>(CollectionName.Users);

    public IMongoCollection<Chat> Chats => _database.GetCollection<Chat>(CollectionName.Chats);

    public IMongoCollection<Message> Messages => _database.GetCollection<Message>(CollectionName.Messages);

    public IMongoCollection<Contact> Contacts => _database.GetCollection<Contact>(CollectionName.Contacts);
}
