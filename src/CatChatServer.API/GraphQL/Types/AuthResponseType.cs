using CatChatServer.Domain.Models.Auth;
using HotChocolate.Types;

namespace CatChatServer.API.GraphQL.Types;

[ExtendObjectType(typeof(BaseType))]
public sealed class AuthResponseType : ObjectType<AuthResponse>
{
    protected override void Configure(IObjectTypeDescriptor<AuthResponse> descriptor)
    {
        descriptor.Field(f => f.AccessToken).Type<StringType>();
        descriptor.Field(f => f.RefreshToken).Type<StringType>();
    }
}
