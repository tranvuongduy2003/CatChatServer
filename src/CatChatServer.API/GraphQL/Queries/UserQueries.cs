using CatChatServer.Domain.Entities;
using CatChatServer.Domain.Repositories;
using HotChocolate;

namespace CatChatServer.API.GraphQL.Queries;

public sealed class UserQueries
{
    public async Task<User?> GetUserById(
        [Service] IUserRepository userRepository, 
        string id)
    {
        return await userRepository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<User>> GetAllUsers(
        [Service] IUserRepository userRepository)
    {
        return await userRepository.GetAllAsync();
    }
}
