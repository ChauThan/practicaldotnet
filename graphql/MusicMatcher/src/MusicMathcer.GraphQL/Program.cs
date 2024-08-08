using MusicMatcher.GraphQL;
using MusicMatcher.GraphQL.Types;
using SpotifyWeb;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient<SpotifyService>();

builder.Services
    .AddGraphQLServer()
        .AddQueryType<Query>()
        .AddMutationType<Mutation>()
        .RegisterService<SpotifyService>();

builder.Services
    .AddCors(o =>
    {
        o.AddDefaultPolicy(builder =>
        {
            builder
                .WithOrigins("https://studio.apollographql.com")
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
    });

var app = builder.Build();

app.UseCors();
app.MapGraphQL();

app.Run();
