using CatChatServer.Domain.Entities;
using CatChatServer.Domain.Enums;
using HotChocolate.Types;

namespace CatChatServer.API.GraphQL.Types;

[ExtendObjectType(typeof(BaseType))]
internal sealed class UserType : ObjectType<User>
{
    protected override void Configure(IObjectTypeDescriptor<User> descriptor)
    {
        descriptor.Field(f => f.Id).Type<NonNullType<IdType>>();
        descriptor.Field(f => f.Username).Type<NonNullType<StringType>>();
        descriptor.Field(f => f.Email).Type<NonNullType<StringType>>();
        descriptor.Field(f => f.ProfilePicture).Type<StringType>();
        descriptor.Field(f => f.Status).Type<EnumType<EUserStatus>>();

        // Hide sensitive fields
        descriptor.Ignore(f => f.PasswordHash);
        descriptor.Ignore(f => f.RefreshToken);
    }
}
