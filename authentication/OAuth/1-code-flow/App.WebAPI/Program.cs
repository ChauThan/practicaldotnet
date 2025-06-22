using App.WebAPI;
using App.Infrastructure.Identity.Extensions;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddOpenApi();
builder.Services.AddControllersWithViews();
builder.Services.AddIdentityInfrastructure(builder.Configuration);
builder.Services.AddIdentityServerConfig();

// Add health checks
builder.Services.AddHealthChecks();

// Configure authentication and authorization
var authorityUrl = builder.Configuration["Jwt:Authority"] ?? "https://localhost:7001";

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.Authority = authorityUrl;
        options.RequireHttpsMetadata = false; // For demo only
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = true,
            ValidAudiences = ["api.read", "api.write"]
        };
    });

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("ApiReadScope", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireClaim("scope", "api.read");
    })
    .AddPolicy("ApiWriteScope", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireClaim("scope", "api.write");
    });

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        policyBuilder => policyBuilder.AllowAnyOrigin()
                                      .AllowAnyMethod()
                                      .AllowAnyHeader());
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Map health check endpoint
app.MapHealthChecks("/health");

app.UseHttpsRedirection();

await app.InitializeDatabaseAsync();

app.Run();