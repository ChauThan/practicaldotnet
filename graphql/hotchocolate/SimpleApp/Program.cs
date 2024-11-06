using Proj;
using Proj.Types;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<DbContext>();

builder.Services
    .AddGraphQLServer()
    .AddInMemorySubscriptions()
    .AddQueryType<QueryType>()
    .AddMutationType<MutationType>()
        .AddMutationConventions()
    .AddSubscriptionType<SubscriptionType>();

var app = builder.Build();

app.UseWebSockets();
app.MapGraphQL();


app.Run();
