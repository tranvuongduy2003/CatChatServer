namespace CatChatServer.API.Extensions;

internal static class AppBuilderExtensions
{
    public static WebApplicationBuilder AddAppConfigurations(this WebApplicationBuilder builder)
    {
        string environmentName = builder.Environment.EnvironmentName;
        
        builder.Configuration
            .AddJsonFile("appsettings.json", false, true)
            .AddJsonFile($"appsettings.{environmentName}.json", true, true)
            .AddEnvironmentVariables();

        return builder;
    }
}
