using Proj;
using Proj.Types;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<DbContext>();

builder.Services
    .AddGraphQLServer()
    .AddQueryType<QueryType>();

var app = builder.Build();

app.MapGraphQL();

app.Run();
