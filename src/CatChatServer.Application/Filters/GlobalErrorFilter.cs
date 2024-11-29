using HotChocolate;
using Microsoft.Extensions.Logging;

namespace CatChatServer.Application.Filters;

public sealed class GlobalErrorFilter(ILogger<GlobalErrorFilter> logger) : IErrorFilter
{
    public IError OnError(IError error)
    {
        // Log the error
        logger.LogError("GraphQL Error: {ErrorMessage}", error.Message);

        // Remove sensitive information in production
        if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Production")
        {
            return error.WithMessage("An unexpected error occurred.")
                .RemoveExtensions();
        }

        return error.SetExtension("timestamp", DateTime.UtcNow);
    }
}
