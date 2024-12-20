using System.Text;
using CatChatServer.API.GraphQL;
using CatChatServer.API.GraphQL.Mutations;
using CatChatServer.API.GraphQL.Queries;
using CatChatServer.API.GraphQL.Types;
using CatChatServer.API.Interceptors;
using CatChatServer.Application.Filters;
using CatChatServer.Application.Services;
using CatChatServer.Domain.Common.Settings;
using CatChatServer.Domain.Interfaces;
using CatChatServer.Domain.Repositories;
using CatChatServer.Infrastructure.Common;
using CatChatServer.Infrastructure.Data;
using CatChatServer.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Minio;
using MongoDB.Driver;
using KeyNotFoundException = GreenDonut.KeyNotFoundException;

namespace CatChatServer.API.Extensions;

internal static class ServiceExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .ConfigureAppSettings(configuration)
            .ConfigureCors()
            .ConfigureAuthentication()
            .ConfigureAuthorization()
            .ConfigureMinio()
            .ConfigureMongoDB()
            .ConfigureGraphQL()
            .ConfigureDependencyInjection();

        return services;
    }

    private static IServiceCollection ConfigureCors(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("AllowReactApp", builder =>
            {
                builder.AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowAnyOrigin();
            });
        });

        return services;
    }

    private static IServiceCollection ConfigureAuthentication(this IServiceCollection services)
    {
        JwtSettings jwtSettings = services.GetOptions<JwtSettings>(nameof(JwtSettings));

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key))
                };
                options.SaveToken = true;
            });

        return services;
    }

    private static IServiceCollection ConfigureAuthorization(this IServiceCollection services) =>
        services.AddAuthorization();

    private static IServiceCollection ConfigureGraphQL(this IServiceCollection services)
    {
        services
            .AddGraphQLServer()
            .AddAuthorization()
            .AddType<BaseType>()
            .AddQueryType<QueryBaseType>()
            .AddMutationType<MutationBaseType>()
            .AddTypeExtension<UserType>()
            .AddTypeExtension<UserQueries>()
            .AddTypeExtension<UserMutations>()
            .AddTypeExtension<AuthResponseType>()
            .AddTypeExtension<AuthQueries>()
            .AddTypeExtension<AuthMutations>()
            .AddHttpRequestInterceptor<HttpRequestInterceptor>()
            .AddErrorFilter<GlobalErrorFilter>();

        return services;
    }

    private static IServiceCollection ConfigureAppSettings(this IServiceCollection services,
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

        JwtSettings jwtSettings = configuration
            .GetSection(nameof(JwtSettings))
            .Get<JwtSettings>();
        services.AddSingleton<JwtSettings>(jwtSettings!);

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

        string mongoConnectionString =
            $"mongodb://{mongoDBSettings.User}:{mongoDBSettings.Password}@{mongoDBSettings.Host}:{mongoDBSettings.Port}/{mongoDBSettings.DatabaseName}?authSource=admin";

#pragma warning disable CA2000
        var mongoClient = new MongoClient(mongoConnectionString);
#pragma warning restore CA2000

        services
            .AddSingleton<IMongoClient>(mongoClient)
            .AddScoped(sp =>
                sp.GetRequiredService<IMongoClient>().StartSession());

        services.AddSingleton<MongoDBContext>();

        return services;
    }


    private static IServiceCollection ConfigureMinio(this IServiceCollection services)
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

    private static IServiceCollection ConfigureDependencyInjection(this IServiceCollection services)
    {
        services
            .AddSingleton<IFileService, MinioStorageService>()
            .AddSingleton<IPasswordHasher, PasswordHasherService>()
            .AddSingleton<ITokenService, TokenService>();

        services
            .AddScoped(typeof(IRepository<>), typeof(BaseRepository<>))
            .AddScoped<IUserRepository, UserRepository>()
            .AddScoped<IAuthService, AuthService>();

        return services;
    }

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
