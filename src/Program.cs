using CatChatServer.Extensions;
using CatChatServer.Filters;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.AddAppConfigurations();

builder.Host.ConfigureLogging(builder.Environment, builder.Configuration);

builder.Services.AddServices(builder.Configuration);

builder.AddGraphQL()
    // .AddTypes()
    .AddErrorFilter<AggregateErrorFilter>();

WebApplication app = builder.Build();

app.MapGraphQL();

app.RunWithGraphQLCommands(args);
