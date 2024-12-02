using System.Security.Claims;
using CatChatServer.Domain.Entities;
using CatChatServer.Domain.Repositories;
using HotChocolate;
using HotChocolate.Authorization;
using HotChocolate.Types;
using Microsoft.IdentityModel.JsonWebTokens;
using MongoDB.Bson;

namespace CatChatServer.API.GraphQL.Queries;

[ExtendObjectType(typeof(QueryBaseType))]
public sealed class AuthQueries
{
    [Authorize]
    public async Task<User> GetMe(
        ClaimsPrincipal claimsPrincipal,
        [Service] IUserRepository userRepository
    )
    {
       
        string userId = claimsPrincipal.Claims
            .ToList()
            .Find(c => c.Type ==JwtRegisteredClaimNames.Sub)?.Value ?? "";

        return await userRepository.GetByIdAsync(userId);
    }
}
