using CatChatServer.Domain.Entities;
using CatChatServer.Domain.Repositories;
using HotChocolate;
using HotChocolate.Authorization;
using HotChocolate.Types;

namespace CatChatServer.API.GraphQL.Queries;

[ExtendObjectType(typeof(QueryBaseType))]
internal sealed class UserQueries
{
    [Authorize]
    public async Task<User?> GetUserById(
        [Service] IUserRepository userRepository,
        string id)
    {
        return await userRepository.GetByIdAsync(id);
    }

    [Authorize]
    public async Task<IEnumerable<User>> GetAllUsers(
        [Service] IUserRepository userRepository)
    {
        return await userRepository.GetAllAsync();
    }
}
