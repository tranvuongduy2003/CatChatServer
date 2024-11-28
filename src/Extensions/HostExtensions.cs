using System.Globalization;
using Serilog;

namespace CatChatServer.Extensions;

public static class HostExtensions
{
    public static IHostBuilder ConfigureLogging(this IHostBuilder builder, IWebHostEnvironment environment,
        IConfiguration configuration)
    {
        string environmentName = environment.EnvironmentName ?? "Development";
        string serverUrl = configuration.GetValue<string>("SeqConfiguration:ServerUrl") ?? "";


        builder.UseSerilog((context, config) =>
        {
            config
                .WriteTo.Debug(formatProvider: CultureInfo.InvariantCulture)
                .WriteTo.Seq(serverUrl)
                .WriteTo.Console(outputTemplate:
                    "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}",
                    formatProvider: CultureInfo.InvariantCulture)
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .Enrich.WithProperty("Environment", environmentName)
                .ReadFrom.Configuration(context.Configuration);
        });

        return builder;
    }
}
