using CatChatServer.API.Extensions;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.AddAppConfigurations();

builder.Host.ConfigureLogging(builder.Environment, builder.Configuration);

builder.Services.AddServices(builder.Configuration);

WebApplication app = builder.Build();

app.MapGraphQL();
app.MapGraphQLWebSocket();

app.Run();
