using CatChatServer.Domain.Entities;
using CatChatServer.Infrastructure.Repositories;
using HotChocolate;

namespace CatChatServer.API.GraphQL.Queries;

public sealed class UserQueries
{
    public async Task<User?> GetUserById(
        [Service] UserRepository userRepository, 
        string id)
    {
        return await userRepository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<User>> GetAllUsers(
        [Service] UserRepository userRepository)
    {
        return await userRepository.GetAllAsync();
    }
}
