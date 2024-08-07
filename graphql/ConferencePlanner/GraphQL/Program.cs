using ConferencePlanner.GraphQL;
using ConferencePlanner.GraphQL.Data;
using ConferencePlanner.GraphQL.DataLoader;
using ConferencePlanner.GraphQL.Speakers;
using ConferencePlanner.GraphQL.Sessions;
using ConferencePlanner.GraphQL.Tracks;
using ConferencePlanner.GraphQL.Types;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var services = builder.Services;
services.AddPooledDbContextFactory<ApplicationDbContext>(options => options.UseSqlite("Data Source=conferences.db"));

services
    .AddGraphQLServer()
    .RegisterDbContext<ApplicationDbContext>(DbContextKind.Pooled)
    .AddQueryType(d => d.Name("Query"))
        .AddTypeExtension<AttendeeQueries>()
        .AddTypeExtension<SessionQueries>()
        .AddTypeExtension<SpeakerQueries>()
        .AddTypeExtension<TrackQueries>()
    .AddMutationType(d => d.Name("Mutation"))
        .AddTypeExtension<AttendeeMutations>()
        .AddTypeExtension<SpeakerMutations>()
        .AddTypeExtension<SessionMutations>()
        .AddTypeExtension<TrackMutations>()
    .AddSubscriptionType(d => d.Name("Subscription"))
        .AddTypeExtension<AttendeeSubscriptions>()
        .AddTypeExtension<SessionSubscriptions>()
    .AddType<AttendeeType>()
    .AddType<SessionType>()
    .AddType<SpeakerType>()
    .AddType<InstructorType>()
    .AddType<PresenterType>()
    .AddGlobalObjectIdentification(true)
    .AddFiltering()
    .AddSorting()
    .AddInMemorySubscriptions()
    .AddDataLoader<SpeakerByIdDataLoader>()
    .AddDataLoader<SessionByIdDataLoader>()
    .AddDataLoader<AttendeeByIdDataLoader>()
    .AddDataLoader<TrackByIdDataLoader>()
    .TryAddTypeInterceptor<NodeResolverTypeInterceptor>()
    ;

var app = builder.Build();
app.MapGet("/", () => "Hello World!");

app.UseWebSockets();
app.MapGraphQL();

app.Run();
