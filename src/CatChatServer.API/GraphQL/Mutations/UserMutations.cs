using CatChatServer.Domain.Entities;
using CatChatServer.Domain.Enums;
using CatChatServer.Domain.Repositories;
using HotChocolate;
using HotChocolate.Authorization;
using HotChocolate.Types;

namespace CatChatServer.API.GraphQL.Mutations;

[ExtendObjectType(typeof(MutationBaseType))]
public sealed class UserMutations
{
    [Authorize]
    public async Task<User> CreateUser(
        [Service] IUserRepository userRepository,
        string username,
        string email,
        string password)
    {
        var user = new User
        {
            Username = username,
            Email = email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password)
        };

        return await userRepository.AddAsync(user);
    }

    [Authorize]
    public async Task<User> UpdateUserStatus(
        [Service] IUserRepository userRepository,
        string userId,
        EUserStatus status)
    {
        User user = await userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            throw new GraphQLException("User not found");
        }

        user.Status = status;
        await userRepository.UpdateAsync(user);
        return user;
    }
}
