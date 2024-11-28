using CatChatServer.Abstractions.Services;
using CatChatServer.Models.Settings;
using CatChatServer.Services;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Minio;
using MongoDB.Driver;
using MongoDB.Driver.Core.Configuration;
using KeyNotFoundException = GreenDonut.KeyNotFoundException;

namespace CatChatServer.Extensions;

internal static class ServiceExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .ConfigureAppSettings(configuration)
            .ConfigureMinio()
            .ConfigureMongoDB()
            .ConfigureDependencyInjection();

        return services;
    }

    public static IServiceCollection ConfigureAppSettings(this IServiceCollection services,
        IConfiguration configuration)
    {
        MinioStorageSettings minioStorageSettings = configuration
            .GetSection(nameof(MinioStorageSettings))
            .Get<MinioStorageSettings>();
        services.AddSingleton<MinioStorageSettings>(minioStorageSettings!);

        MongoDBSettings mongoDBSettings = configuration
            .GetSection(nameof(MongoDBSettings))
            .Get<MongoDBSettings>();
        services.AddSingleton<MongoDBSettings>(mongoDBSettings!);

        return services;
    }

    private static IServiceCollection ConfigureMongoDB(this IServiceCollection services)
    {
        MongoDBSettings mongoDBSettings = services.GetOptions<MongoDBSettings>(nameof(MongoDBSettings));
        if (mongoDBSettings == null
            || string.IsNullOrEmpty(mongoDBSettings.Host)
            || string.IsNullOrEmpty(mongoDBSettings.User)
            || string.IsNullOrEmpty(mongoDBSettings.DatabaseName)
            || string.IsNullOrEmpty(mongoDBSettings.Password))
        {
            throw new KeyNotFoundException("MongoDBSettings is not configured");
        }

        var mongoClientSettings = new MongoClientSettings
        {
            Scheme = ConnectionStringScheme.MongoDB,
            Server = new MongoServerAddress(mongoDBSettings.Host, mongoDBSettings.Port),
            Credential = MongoCredential.CreateCredential(mongoDBSettings.DatabaseName, mongoDBSettings.User,
                mongoDBSettings.Password)
        };

        using var mongoClient = new MongoClient(mongoClientSettings);
        services.TryAddSingleton<IMongoClient>(mongoClient);

        return services;
    }


    public static IServiceCollection ConfigureMinio(this IServiceCollection services)
    {
        MinioStorageSettings minioStorageSettings = services.GetOptions<MinioStorageSettings>("MinioStorageSettings");
        if (minioStorageSettings == null
            || string.IsNullOrEmpty(minioStorageSettings.Endpoint)
            || string.IsNullOrEmpty(minioStorageSettings.AccessKey)
            || string.IsNullOrEmpty(minioStorageSettings.SecretKey))
        {
            throw new Exception("MinioStorageSettings is not configured properly!");
        }

        services.AddMinio(configureClient => configureClient
            .WithEndpoint(minioStorageSettings.Endpoint)
            .WithCredentials(minioStorageSettings.AccessKey, minioStorageSettings.SecretKey)
            .WithSSL(minioStorageSettings.Secure)
            .Build());

        return services;
    }

    public static IServiceCollection ConfigureDependencyInjection(this IServiceCollection services) =>
        services.AddSingleton<IFileService, MinioStorageService>();

    public static T GetOptions<T>(this IServiceCollection services, string sectionName) where T : new()
    {
        using ServiceProvider serviceProvider = services.BuildServiceProvider();
        IConfiguration configuration = serviceProvider.GetRequiredService<IConfiguration>();
        IConfigurationSection section = configuration.GetSection(sectionName);

        var options = new T();
        section.Bind(options);

        return options;
    }
}
