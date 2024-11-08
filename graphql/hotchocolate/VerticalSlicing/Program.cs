using SimpleApp;
using SimpleApp.Common;
using SimpleApp.Features.RootApp;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<DbContext>();

builder.Services
    .AddGraphQLServer()
    .AddQueryType<QueryType>()
    .AddMutationType<MutationType>();

var app = builder.Build();

app.MapGraphQL();

app.Run();
