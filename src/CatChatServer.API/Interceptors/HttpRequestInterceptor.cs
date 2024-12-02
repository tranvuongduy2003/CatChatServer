using System.Security.Claims;
using CatChatServer.Domain.Interfaces;
using HotChocolate.AspNetCore;
using HotChocolate.Execution;
using Microsoft.IdentityModel.Tokens;

namespace CatChatServer.API.Interceptors;

public class HttpRequestInterceptor : DefaultHttpRequestInterceptor
{
    private readonly ITokenService _tokenService;

    public HttpRequestInterceptor(ITokenService tokenService)
    {
        _tokenService = tokenService;
    }

    public override ValueTask OnCreateAsync(HttpContext context,
        IRequestExecutor requestExecutor, OperationRequestBuilder requestBuilder,
        CancellationToken cancellationToken)
    {
        string authorization = context.Request.Headers.Authorization.ToString();
        if (!authorization.IsNullOrEmpty() && authorization.Contains("Bearer"))
        {
            string accessToken = authorization.Split(' ')[1];
            ClaimsIdentity identity = _tokenService.GetPrincipalFromToken(accessToken).GetAwaiter().GetResult();
            context.User.AddIdentity(identity!);
        }

        return base.OnCreateAsync(context, requestExecutor, requestBuilder,
            cancellationToken);
    }
}
