using CatChatServer.Domain.Entities;
using CatChatServer.Domain.Repositories;
using CatChatServer.Infrastructure.Common;
using CatChatServer.Infrastructure.Data;
using MongoDB.Driver;

namespace CatChatServer.Infrastructure.Repositories;

public class UserRepository(MongoDBContext context) : BaseRepository<User>(context.Users), IUserRepository
{
    public async Task<User?> GetUserByEmailAsync(string email) =>
        await _collection.Find(u => u.Email == email).FirstOrDefaultAsync();

    public async Task<User?> GetUserByUsernameAsync(string username) =>
        await _collection.Find(u => u.Username == username).FirstOrDefaultAsync();
}
