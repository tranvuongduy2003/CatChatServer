using CatChatServer.Domain.Entities;
using CatChatServer.Domain.Interfaces;

namespace CatChatServer.Domain.Repositories;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetUserByEmailAsync(string email);

    Task<User?> GetUserByUsernameAsync(string username);
}
