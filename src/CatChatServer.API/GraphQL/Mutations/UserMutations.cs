using CatChatServer.Domain.Entities;
using CatChatServer.Domain.Enums;
using CatChatServer.Infrastructure.Repositories;
using HotChocolate;

namespace CatChatServer.API.GraphQL.Mutations;

public sealed class UserMutations
{
    public async Task<User> CreateUser(
        [Service] UserRepository userRepository,
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

    public async Task<User> UpdateUserStatus(
        [Service] UserRepository userRepository,
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
